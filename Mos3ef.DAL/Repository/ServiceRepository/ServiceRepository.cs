using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mos3ef.DAL.Database;
using Mos3ef.DAL.Models;
using Mos3ef.DAL.Enum;

namespace Mos3ef.DAL.Repository.ServiceRepository
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly ApplicationDbContext _context;

        public ServiceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Service>> GetServices()
        {
            return await _context.Services.ToListAsync();
        }

        public async Task<Service> GetService(int id)
        {
            return await _context.Services.FindAsync(id);
        }

        public async Task<IEnumerable<Review>> GetServiceReviews(int id)
        {
            return await _context.Reviews.Where(r => r.ServiceId == id).ToListAsync();
        }

        public async Task<Hospital?> GetServiceHospital(int id)
        {
            var service = await _context.Services.Include(s => s.Hospital).FirstOrDefaultAsync(s => s.ServiceId == id);
            return service?.Hospital;
        }

        public async Task<IEnumerable<Service>> SearchServicesAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return Enumerable.Empty<Service>();
            }

            var lowerCaseKeyword = keyword.ToLower();

            var query = _context.Services
                .Include(s => s.Hospital)
                .Include(s => s.Reviews)
                .Where(s => s.Name != null && s.Name.ToLower().Contains(lowerCaseKeyword) ||
                            s.Description != null && s.Description.ToLower().Contains(lowerCaseKeyword));

            var sortedQuery = query
                .OrderByDescending(s => s.Availability != null && s.Availability.ToLower() == "available");

            return await sortedQuery.ToListAsync();
        }

        public async Task<(IEnumerable<Service> Services, int TotalCount)> FilterServicesAsync(
            bool? hasEmergency, bool? hasIcu, bool? hasNicu, string? hospitalName, 
            decimal? maxPrice, string? sortBy, bool isAscending,
            int? minRating, string? region, bool onlyAvailable,
            double? userLatitude, double? userLongitude, double? radiusKm,
            string? keyword, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Services
                .Include(s => s.Hospital)
                .Include(s => s.Reviews)
                .AsQueryable();

            // Apply filters in database
            if (hasIcu == true)
            {
                query = query.Where(s => s.Category == CategoryType.ICU);
            }

            if (hasEmergency == true)
            {
                query = query.Where(s => s.Category == CategoryType.EmergencyRoom);
            }

            if (onlyAvailable)
            {
                query = query.Where(s => s.Availability != null && s.Availability.ToLower() == "available");
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var k = keyword.ToLower();
                query = query.Where(s => s.Name != null && s.Name.ToLower().Contains(k) || 
                                        s.Description != null && s.Description.ToLower().Contains(k));
            }

            if (!string.IsNullOrWhiteSpace(hospitalName))
            {
                query = query.Where(s => s.Hospital.Name.ToLower().Contains(hospitalName.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(region))
            {
                query = query.Where(s => s.Hospital.Region != null && s.Hospital.Region.ToLower().Contains(region.ToLower()));
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(s => s.Price <= maxPrice.Value);
            }


            // Filter by distance and radius in database (approximate using bounding box for performance)
            // For exact distance, we'll calculate after fetching but filter radius in query
            if (userLatitude.HasValue && userLongitude.HasValue && radiusKm.HasValue)
            {
                // Approximate bounding box (1 degree ≈ 111 km)
                var latDelta = radiusKm.Value / 111.0;
                var lonDelta = radiusKm.Value / (111.0 * Math.Cos(userLatitude.Value * Math.PI / 180.0));
                
                query = query.Where(s => s.Hospital.Latitude.HasValue && 
                                       s.Hospital.Longitude.HasValue &&
                                       s.Hospital.Latitude >= userLatitude.Value - latDelta &&
                                       s.Hospital.Latitude <= userLatitude.Value + latDelta &&
                                       s.Hospital.Longitude >= userLongitude.Value - lonDelta &&
                                       s.Hospital.Longitude <= userLongitude.Value + lonDelta);
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Apply sorting in database
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "price":
                        query = isAscending ? query.OrderBy(s => s.Price) : query.OrderByDescending(s => s.Price);
                        break;
                    case "name":
                        query = isAscending ? query.OrderBy(s => s.Name) : query.OrderByDescending(s => s.Name);
                        break;
                    case "rating":
                        // Sort by average rating - calculated in database
                        query = isAscending 
                            ? query.OrderBy(s => s.Reviews != null && s.Reviews.Count > 0 
                                ? s.Reviews.Average(r => r.Rating) : 0)
                            : query.OrderByDescending(s => s.Reviews != null && s.Reviews.Count > 0 
                                ? s.Reviews.Average(r => r.Rating) : 0);
                        break;
                    case "distance":
                        if (userLatitude.HasValue && userLongitude.HasValue)
                        {
                            // For distance sorting, we'll need to calculate after fetching
                            // But we can still apply pagination after distance calculation
                            // For now, sort by a combination of lat/lon proximity
                            query = query.OrderBy(s => 
                                Math.Abs((s.Hospital.Latitude ?? 0) - userLatitude.Value) +
                                Math.Abs((s.Hospital.Longitude ?? 0) - userLongitude.Value));
                        }
                        break;
                    default:
                        query = query.OrderBy(s => s.Name);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(s => s.Name);
            }

            // Fetch all matching services (before pagination) to apply rating filter
            var allServices = await query.ToListAsync();

            // Apply rating filter in memory (EF Core limitation)
            if (minRating.HasValue)
            {
                allServices = allServices.Where(s => 
                    s.Reviews != null && 
                    s.Reviews.Count > 0 && 
                    s.Reviews.Average(r => r.Rating) >= minRating.Value).ToList();
            }

            // Recalculate total count after rating filter
            totalCount = allServices.Count;

            // Apply distance calculation and sorting if needed
            if (!string.IsNullOrWhiteSpace(sortBy) && sortBy.ToLower() == "distance" && 
                userLatitude.HasValue && userLongitude.HasValue)
            {
                Func<double?, double?, double?, double?, double?> dist = (lat1, lon1, lat2, lon2) =>
                {
                    if (!lat1.HasValue || !lon1.HasValue || !lat2.HasValue || !lon2.HasValue) return double.MaxValue;
                    var dLat = (lat2.Value - lat1.Value) * Math.PI / 180.0;
                    var dLon = (lon2.Value - lon1.Value) * Math.PI / 180.0;
                    var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + 
                            Math.Cos(lat1.Value * Math.PI / 180.0) * Math.Cos(lat2.Value * Math.PI / 180.0) * 
                            Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
                    var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
                    return 6371.0 * c;
                };

                allServices = allServices
                    .Select(s => new { Service = s, Distance = dist(userLatitude, userLongitude, s.Hospital.Latitude, s.Hospital.Longitude) })
                    .OrderBy(x => x.Distance)
                    .Select(x => x.Service)
                    .ToList();
            }

            // Apply pagination after all filters and sorting
            var services = allServices
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return (services, totalCount);
        }

        public async Task<Service?> GetByIdAsync(int serviceId)
        {
            return await _context.Services.Include(s => s.Hospital).Include(s => s.Reviews).FirstOrDefaultAsync(s => s.ServiceId == serviceId);
        }
    }
}

