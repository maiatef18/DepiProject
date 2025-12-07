using Mos3ef.BLL.Dtos.Common;
using Mos3ef.BLL.Dtos.Patient;
using Mos3ef.BLL.Dtos.Services;
using Mos3ef.DAL.Models;
using System;

namespace Mos3ef.BLL.Manager.PatientManager
{
    /// <summary>
    /// Interface for patient-related business operations.
    /// Methods throw exceptions for error cases instead of returning null/false.
    /// </summary>
    public interface IPatientManager
    {
        /// <exception cref="Mos3ef.Api.Exceptions.NotFoundException">Thrown when patient not found</exception>
        Task<PatientReadDto> GetPatientByIdAsync(int id);
        
        /// <exception cref="Mos3ef.Api.Exceptions.NotFoundException">Thrown when patient not found</exception>
        Task<PatientReadDto> GetPatientByUserIdAsync(string userId);
        
        /// <exception cref="Mos3ef.Api.Exceptions.NotFoundException">Thrown when patient not found</exception>
        Task UpdatePatientAsync(int id, PatientUpdateDto patientUpdateDto, string? imagePath = null);
        
        Task<IEnumerable<ServiceReadDto>> GetSavedServicesAsync(int patientId);
        Task<PagedResult<ServiceReadDto>> GetSavedServicesPagedAsync(int patientId, int pageNumber, int pageSize);
        
        /// <exception cref="Mos3ef.Api.Exceptions.NotFoundException">Thrown when service not found</exception>
        Task SaveServiceAsync(int patientId, int serviceId);
        
        /// <exception cref="Mos3ef.Api.Exceptions.NotFoundException">Thrown when saved service link not found</exception>
        Task RemoveSavedServiceAsync(int patientId, int serviceId);
    }
}
