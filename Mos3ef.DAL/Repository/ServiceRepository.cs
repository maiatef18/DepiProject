using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mos3ef.DAL.Database;
using Mos3ef.DAL.Models;

namespace Mos3ef.DAL.Repository
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly ApplicationDbContext _context;

        public ServiceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Service>> SearchServicesAsync(string keyword)
        {
            // Make sure keyword is not null or empty
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return Enumerable.Empty<Service>();
            }

            var lowerCaseKeyword = keyword.ToLower();

            // Search in Service Name and Description
            return await _context.Services
                .Where(s => s.Name.ToLower().Contains(lowerCaseKeyword) ||
                            s.Description.ToLower().Contains(lowerCaseKeyword))
                .ToListAsync();
        }
    }
}
