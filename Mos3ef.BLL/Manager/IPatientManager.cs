using Mos3ef.BLL.Dtos.Patient;
using Mos3ef.DAL.Models;

namespace Mos3ef.BLL.Manager
{
    public interface IPatientManager
    {
        Task<Patient?> GetPatientByIdAsync(int id);
        Task<bool> UpdatePatientAsync(int id, PatientUpdateDto patientUpdateDto);
    }
}