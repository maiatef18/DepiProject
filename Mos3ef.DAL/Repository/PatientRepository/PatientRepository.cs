using Microsoft.EntityFrameworkCore;
using Mos3ef.DAL.Database;
using Mos3ef.DAL.Models;

namespace Mos3ef.DAL.Repository
{
    public class PatientRepository : IPatientRepository
    {
        private readonly ApplicationDbContext _context;

        public PatientRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddSavedServiceAsync(SavedService savedService)
        {
            await _context.SavedServices.AddAsync(savedService);
        }

        public async Task<SavedService?> FindSavedServiceAsync(int patientId, int serviceId)
        {
            return await _context.SavedServices
                .FirstOrDefaultAsync(ss => ss.PatientId == patientId && ss.ServiceId == serviceId);
        }

        public async Task<Patient?> GetPatientByIdAsync(int id)
        {
            return await _context.Patients
                .Include(p => p.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.PatientId == id);
        }

        public async Task<Patient?> GetPatientByIdForUpdateAsync(int id)
        {
            // No AsNoTracking - entity will be tracked for updates
            return await _context.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PatientId == id);
        }

        public async Task<Patient?> GetPatientByUserIdAsync(string userId)
        {
            return await _context.Patients
                .Include(p => p.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<IEnumerable<SavedService>> GetSavedServicesAsync(int patientId)
        {
            return await _context.SavedServices
                .Where(ss => ss.PatientId == patientId)
                .Include(ss => ss.Service)
                .ToListAsync();
        }

        public async Task<(IEnumerable<SavedService> SavedServices, int TotalCount)> GetSavedServicesPagedAsync(int patientId, int pageNumber, int pageSize)
        {
            var query = _context.SavedServices
                .Where(ss => ss.PatientId == patientId)
                .Include(ss => ss.Service);

            var totalCount = await query.CountAsync();

            var savedServices = await query
                .OrderByDescending(ss => ss.Saved_Date)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (savedServices, totalCount);
        }

        public void RemoveSavedService(SavedService savedService)
        {
            _context.SavedServices.Remove(savedService);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}