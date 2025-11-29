using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Mos3ef.DAL.Database;
using Mos3ef.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mos3ef.DAL.Repository.HospitalRepository
{
    public class HospitalRepository : IHospitalRepository
    {
        private readonly ApplicationDbContext _Context;
        public HospitalRepository(ApplicationDbContext applicationDbContext)
        {
            _Context = applicationDbContext;
        }

        public async Task<int> GetHospitalIdByUserIdAsync(string userId)
        {
            var hospital = await _Context.Hospitals
                                         .FirstOrDefaultAsync(h => h.UserId == userId);

            return hospital?.HospitalId ?? 0;
        }

        public async Task<int> AddAsync(Hospital hospital)
        {
            await _Context.Hospitals.AddAsync(hospital);
            await _Context.SaveChangesAsync();
            return hospital.HospitalId;
        }

        public async Task UpdateAsync()
        {
            await _Context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Hospital hospital)
        {
            _Context.Hospitals.Remove(hospital);
            await _Context.SaveChangesAsync();
        }

        public async Task<Hospital?> GetAsync(string id)
        {
            return await _Context.Hospitals
                .Include(h => h.Services)
                .FirstOrDefaultAsync(h => h.UserId == id);
        }

        public async Task<Hospital?> GetHospitalAsync(string id)
        {
            return await _Context.Hospitals
                .Include(h => h.Services)
                .FirstOrDefaultAsync(h => h.UserId == id);
        }
        public async Task<IEnumerable<Hospital>> GetAllAsync()
        {
            return await _Context.Hospitals
                .Include(h => h.Services)
                .ToListAsync();
        }

        public async Task<Service?> GetServiceAsync(string userId, int serviceId)
        {
            var hospital = await _Context.Hospitals
                                         .FirstOrDefaultAsync(h => h.UserId == userId);

            if (hospital == null)
                return null;

            return await _Context.Services
                .FirstOrDefaultAsync(s => s.ServiceId == serviceId
                                       && s.HospitalId == hospital.HospitalId);
        }



        public async Task<int> AddServiceAsync(Service service)
        {
            await _Context.Services.AddAsync(service);
            await _Context.SaveChangesAsync();
            return service.ServiceId;
        }

        public async Task UpdateServiceAsync()
        {
            await _Context.SaveChangesAsync();
        }

        public async Task DeleteServiceAsync(Service service)
        {
            _Context.Services.Remove(service);
            await _Context.SaveChangesAsync();
        }


        public async Task<IEnumerable<Review>> GetServicesReviewsAsync(int hospitalId)
        {
            return await _Context.Reviews
                .Include(r => r.Service)
                .Where(r => r.Service.HospitalId == hospitalId)
                .ToListAsync();
        }

        public async Task<(int ServicesCount, int ReviewsCount, double AvgRating)> GetDashboardStatsAsync(int hospitalId)
        {
            int servicesCount = await _Context.Services
                .CountAsync(s => s.HospitalId == hospitalId);

            int reviewsCount = await _Context.Reviews
                .CountAsync(r => r.Service.HospitalId == hospitalId);

            double avgRating = await _Context.Reviews
                .Where(r => r.Service.HospitalId == hospitalId)
                .Select(r => (double?)r.Rating)
                .AverageAsync() ?? 0;

            return (servicesCount, reviewsCount, avgRating);
        }
    }

}

