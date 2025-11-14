using Mos3ef.DAL.Enum;
using Mos3ef.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mos3ef.DAL.Repository.ServiceRepository
{
    public interface IServiceRepository
    {
        Task<IEnumerable<Service>> GetServices();
        Task<Service> GetService(int id);
        Task<IEnumerable<Review>> GetServiceReviews(int id);
        Task<Hospital?> GetServiceHospital(int id);
        Task<IEnumerable<Service>> SearchServicesAsync(string keyword);
        Task<(IEnumerable<Service> Services, int TotalCount)> FilterServicesAsync(
            bool? hasEmergency, bool? hasIcu, bool? hasNicu, string? hospitalName,
            decimal? maxPrice, string? sortBy, bool isAscending,
            int? minRating, string? region, bool onlyAvailable,
            double? userLatitude, double? userLongitude, double? radiusKm,
            string? keyword, int pageNumber = 1, int pageSize = 10);

        Task<Service?> GetByIdAsync(int serviceId);
    }
}
