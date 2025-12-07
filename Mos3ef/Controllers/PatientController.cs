using Microsoft.AspNetCore.Mvc;
using Mos3ef.BLL.Dtos.Patient;
using Mos3ef.BLL.Dtos.Common;
using Mos3ef.BLL.Manager.PatientManager;
using Mos3ef.DAL.Models;
using System.Security.Claims;
using Mos3ef.BLL.Dtos.Services;
using Microsoft.AspNetCore.Authorization;
using Mos3ef.BLL.Services;
using Mos3ef.DAL.Wapper;
using Mos3ef.Api.Exceptions;

namespace Mos3ef.Api.Controllers
{
    /// <summary>
    /// Controller for managing patient profiles and saved services.
    /// 
    /// Following Clean Architecture:
    /// - Controller is thin - only handles HTTP concerns
    /// - Business logic and validation are in the Manager layer
    /// - Exceptions from Manager propagate to GlobalExceptionMiddleware
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class PatientsController : ControllerBase
    {
        private readonly IPatientManager _patientManager;
        private readonly IFileStorageService _fileStorageService;

        public PatientsController(IPatientManager patientManager, IFileStorageService fileStorageService)
        {
            _patientManager = patientManager;
            _fileStorageService = fileStorageService;
        }

        /// <summary>
        /// Retrieves the current patient's ID from claims.
        /// </summary>
        private async Task<int> GetCurrentPatientIdAsync()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedException("User not authenticated.");
            }

            // Manager throws NotFoundException if patient not found
            var patient = await _patientManager.GetPatientByUserIdAsync(userId);
            return patient.PatientId;
        }

        /// <summary>
        /// Get patient by ID. Only accessible by the patient themselves or an Admin.
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Policy = "Patient")]
        public async Task<IActionResult> GetPatientById(int id)
        {
            if (id <= 0)
                throw new BadRequestException("Patient ID must be greater than 0.");

            // Authorization check
            var myPatientId = await GetCurrentPatientIdAsync();
            var isAdmin = User.IsInRole("Admin");
            
            if (!isAdmin && myPatientId != id)
            {
                throw new ForbiddenException("You are not authorized to view this patient's profile.");
            }

            // Manager throws if not found
            var patient = await _patientManager.GetPatientByIdAsync(id);
            return Ok(Response<PatientReadDto>.Success(patient, "Patient retrieved successfully"));
        }

        /// <summary>
        /// Get the logged-in patient's profile.
        /// </summary>
        [HttpGet("my-profile")]
        public async Task<IActionResult> GetMyProfile()
        {
            var patientId = await GetCurrentPatientIdAsync();
            var patient = await _patientManager.GetPatientByIdAsync(patientId);
            return Ok(Response<PatientReadDto>.Success(patient, "Profile retrieved successfully"));
        }

        /// <summary>
        /// Update the logged-in patient's profile.
        /// </summary>
        [HttpPut("my-profile")]
        public async Task<IActionResult> UpdateMyProfile([FromForm] PatientUpdateDto patientUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                throw new ValidationException(errors);
            }

            var myPatientId = await GetCurrentPatientIdAsync();

            string? imagePath = null;
            string? oldImagePath = null;

            // Handle profile picture upload
            if (patientUpdateDto.ProfilePicture != null)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                if (!_fileStorageService.ValidateFile(patientUpdateDto.ProfilePicture, out string errorMessage, allowedExtensions, maxSizeMB: 5))
                {
                    throw new BadRequestException(errorMessage);
                }

                var currentPatient = await _patientManager.GetPatientByIdAsync(myPatientId);
                if (!string.IsNullOrEmpty(currentPatient.ImageUrl))
                {
                    oldImagePath = currentPatient.ImageUrl;
                }

                imagePath = await _fileStorageService.SaveFileAsync(patientUpdateDto.ProfilePicture, "uploads/profiles");
            }

            // Manager throws if not found or if email already in use
            await _patientManager.UpdatePatientAsync(myPatientId, patientUpdateDto, imagePath);

            // Delete old profile picture AFTER successful update
            if (!string.IsNullOrEmpty(oldImagePath))
            {
                await _fileStorageService.DeleteFileAsync(oldImagePath);
            }

            return Ok(Response<bool>.Success(true, "Profile updated successfully"));
        }

        /// <summary>
        /// Get the logged-in patient's saved services with pagination.
        /// </summary>
        [HttpGet("my-saved-services")]
        public async Task<IActionResult> GetMySavedServices(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1)
                throw new BadRequestException("Page number must be at least 1.");

            if (pageSize < 1 || pageSize > 100)
                throw new BadRequestException("Page size must be between 1 and 100.");

            var patientId = await GetCurrentPatientIdAsync();
            var result = await _patientManager.GetSavedServicesPagedAsync(patientId, pageNumber, pageSize);
            return Ok(Response<PagedResult<ServiceReadDto>>.Success(result, "Saved services retrieved successfully"));
        }

        /// <summary>
        /// Save a service to the patient's favorites list.
        /// </summary>
        [HttpPost("my-saved-services/{serviceId}")]
        public async Task<IActionResult> SaveServiceToMyList(int serviceId)
        {
            if (serviceId <= 0)
                throw new BadRequestException("Service ID must be greater than 0.");

            var myPatientId = await GetCurrentPatientIdAsync();
            
            // Manager throws if service not found
            await _patientManager.SaveServiceAsync(myPatientId, serviceId);
            return Ok(Response<bool>.Success(true, "Service saved successfully"));
        }

        /// <summary>
        /// Remove a service from the patient's saved list.
        /// </summary>
        [HttpDelete("my-saved-services/{serviceId}")]
        public async Task<IActionResult> RemoveServiceFromMyList(int serviceId)
        {
            if (serviceId <= 0)
                throw new BadRequestException("Service ID must be greater than 0.");

            var myPatientId = await GetCurrentPatientIdAsync();
            
            // Manager throws if saved link not found
            await _patientManager.RemoveSavedServiceAsync(myPatientId, serviceId);
            return Ok(Response<bool>.Success(true, "Service removed successfully"));
        }
    }
}

