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

        public async Task<IEnumerable<Service>> SearchByKeywordAsync(string keyword)
        {
            keyword = keyword.ToLower();

            return await _context.Services
                .Include(s => s.Hospital)
                .Include(s => s.Reviews)
                .Where(s =>
                    s.Name.ToLower().Contains(keyword) ||
                    s.Hospital.Name.ToLower().Contains(keyword)
                )
                .ToListAsync();
        }


        public async Task<IEnumerable<Service>> SearchByCategoryAsync(CategoryType category)
        {
            return await _context.Services
                .Include(s => s.Hospital)
                .Include(s => s.Reviews)
                .Where(s => s.Category == category)
                .ToListAsync();
        }


        public async Task<Service?> GetByIdAsync(int serviceId)
        {
            return await _context.Services.Include(s => s.Hospital).Include(s => s.Reviews).FirstOrDefaultAsync(s => s.ServiceId == serviceId);
        }
    }
}

