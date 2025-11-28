using Mos3ef.BLL.Dtos.Common;
using Mos3ef.BLL.Dtos.Patient;
using Mos3ef.BLL.Dtos.Services;
using Mos3ef.DAL.Models;
using System;

namespace Mos3ef.BLL.Manager.PatientManager
{
    public interface IPatientManager
    {
        Task<PatientReadDto?> GetPatientByIdAsync(int id);
        Task<PatientReadDto?> GetPatientByUserIdAsync(string userId);
        Task<bool> UpdatePatientAsync(int id, PatientUpdateDto patientUpdateDto, string? imagePath = null);
        Task<IEnumerable<ServiceReadDto>> GetSavedServicesAsync(int patientId);
        Task<PagedResult<ServiceReadDto>> GetSavedServicesPagedAsync(int patientId, int pageNumber, int pageSize);
        Task<bool> SaveServiceAsync(int patientId, int serviceId);
        Task<bool> RemoveSavedServiceAsync(int patientId, int serviceId);
    }
}