using Mos3ef.DAL.Models;

namespace Mos3ef.DAL.Repository
{
    public interface IPatientRepository
    {
        Task<Patient?> GetPatientByIdAsync(int id);
        Task<Patient?> GetPatientByIdForUpdateAsync(int id); // For update operations - WITH tracking
        Task<Patient?> GetPatientByUserIdAsync(string userId);
        Task<int> SaveChangesAsync();

        Task<IEnumerable<SavedService>> GetSavedServicesAsync(int patientId);
        Task<(IEnumerable<SavedService> SavedServices, int TotalCount)> GetSavedServicesPagedAsync(int patientId, int pageNumber, int pageSize);
        Task<SavedService?> FindSavedServiceAsync(int patientId, int serviceId);
        Task AddSavedServiceAsync(SavedService savedService);
        void RemoveSavedService(SavedService savedService);
    }
}