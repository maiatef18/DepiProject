using Mos3ef.DAL.Models;

namespace Mos3ef.DAL.Repository
{
    public interface IPatientRepository
    {
        Task<Patient?> GetPatientByIdAsync(int id);
        Task<int> SaveChangesAsync();
    }
}