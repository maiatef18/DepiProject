using Mos3ef.BLL.Dtos.Patient;
using Mos3ef.DAL.Models;
using Mos3ef.DAL.Repository;

namespace Mos3ef.BLL.Manager
{
    public class PatientManager : IPatientManager
    {
        private readonly IPatientRepository _patientRepository;

        public PatientManager(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }

        public async Task<Patient?> GetPatientByIdAsync(int id)
        {
            return await _patientRepository.GetPatientByIdAsync(id);
        }

        // Add this new method
        public async Task<bool> UpdatePatientAsync(int id, PatientUpdateDto patientUpdateDto)
        {
            var patient = await _patientRepository.GetPatientByIdAsync(id);

            if (patient == null)
            {
                return false; // Indicates patient was not found
            }

            // Map the fields from the DTO to the model
            // The DbContext is already tracking 'patient' from the GetById call
            patient.Name = patientUpdateDto.Name;
            // Add any other fields you put in your DTO
            // patient.PhoneNumber = patientUpdateDto.PhoneNumber;

            await _patientRepository.SaveChangesAsync();
            return true; // Indicates success
        }
    }
}