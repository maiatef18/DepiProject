using Microsoft.AspNetCore.Mvc;
using Mos3ef.BLL.Dtos.Patient;
using Mos3ef.BLL.Dtos.Common;
using Mos3ef.BLL.Manager.PatientManager;
using Mos3ef.DAL.Models;
using System.Security.Claims;
using Mos3ef.BLL.Dtos.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using Mos3ef.BLL.Services;
using Mos3ef.DAL;

namespace Mos3ef.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class PatientsController : ControllerBase
    {
        private readonly IPatientManager _patientManager;
        private readonly IMemoryCache _cache;
        private readonly IFileStorageService _fileStorageService;
        private int? _cachedPatientId; // Request-level cache

        public PatientsController(IPatientManager patientManager, IMemoryCache cache, IFileStorageService fileStorageService)
        {
            _patientManager = patientManager;
            _cache = cache;
            _fileStorageService = fileStorageService;
        }

        private async Task<int?> GetCurrentPatientIdAsync()
        {
            // Return cached value if already fetched in this request
            if (_cachedPatientId.HasValue)
                return _cachedPatientId;

            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }

            var patient = await _patientManager.GetPatientByUserIdAsync(userId);
            if (patient == null)
            {
                return null;
            }

            // Cache for subsequent calls in this request
            _cachedPatientId = patient.PatientId;
            return _cachedPatientId;
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "Patient")]
        public async Task<IActionResult> GetPatientById(int id)
        {
            // Only allow patients to view their own profile, or admins to view any profile
            var myPatientId = await GetCurrentPatientIdAsync();
            var isAdmin = User.IsInRole("Admin");
            
            // If not admin and trying to view another patient's profile, deny access
            if (!isAdmin && (myPatientId == null || myPatientId.Value != id))
            {
                return Forbid();
            }

            var patient = await _patientManager.GetPatientByIdAsync(id);
            if (patient == null)
            {
                return NotFound("Patient not found.");
            }
            return Ok(patient);
        }

        [HttpGet("my-profile")]
        public async Task<ActionResult<PatientReadDto>> GetMyProfile()
        {
            var patientId = await GetCurrentPatientIdAsync();
            if (patientId == null) return Unauthorized();

            // Check cache first
            var cacheKey = CacheConstant.PatientProfilePrefix + patientId;
            if (!_cache.TryGetValue(cacheKey, out PatientReadDto patient))
            {
                // Not in cache, fetch from database
                patient = await _patientManager.GetPatientByIdAsync(patientId.Value);
                if (patient == null) return NotFound();

                // Store in cache for 5 minutes
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
                
                _cache.Set(cacheKey, patient, cacheOptions);
            }

            return Ok(patient);
        }

        [HttpPut("my-profile")]
        public async Task<IActionResult> UpdateMyProfile([FromForm] PatientUpdateDto patientUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var myPatientId = await GetCurrentPatientIdAsync();
            if (myPatientId == null)
            {
                return Unauthorized();
            }

            string? imagePath = null;
            string? oldImagePath = null;

            // Handle profile picture upload
            if (patientUpdateDto.ProfilePicture != null)
            {
                // Validate file using the service
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                if (!_fileStorageService.ValidateFile(patientUpdateDto.ProfilePicture, out string errorMessage, allowedExtensions, maxSizeMB: 5))
                {
                    return BadRequest(errorMessage);
                }

                // Get current patient to check for existing profile picture
                var currentPatient = await _patientManager.GetPatientByIdAsync(myPatientId.Value);
                
                // Store old image path for deletion AFTER successful update
                if (currentPatient != null && !string.IsNullOrEmpty(currentPatient.ImageUrl))
                {
                    oldImagePath = currentPatient.ImageUrl;
                }

                // Save new file using the service
                imagePath = await _fileStorageService.SaveFileAsync(patientUpdateDto.ProfilePicture, "uploads/profiles");
            }

            try
            {
                var success = await _patientManager.UpdatePatientAsync(myPatientId.Value, patientUpdateDto, imagePath);
                if (!success)
                {
                    return NotFound("Patient profile not found.");
                }

                // Delete old profile picture AFTER successful update
                if (!string.IsNullOrEmpty(oldImagePath))
                {
                    await _fileStorageService.DeleteFileAsync(oldImagePath);
                }

                // Invalidate cache after successful update
                var cacheKey = CacheConstant.PatientProfilePrefix + myPatientId.Value;
                _cache.Remove(cacheKey);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            return NoContent();
        }

        [HttpGet("my-saved-services")]
        public async Task<ActionResult<PagedResult<ServiceReadDto>>> GetMySavedServices(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1)
            {
                return BadRequest("Page number must be at least 1.");
            }

            if (pageSize < 1 || pageSize > 100)
            {
                return BadRequest("Page size must be between 1 and 100.");
            }

            var patientId = await GetCurrentPatientIdAsync();
            if (patientId == null) return Unauthorized();

            var result = await _patientManager.GetSavedServicesPagedAsync(patientId.Value, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpPost("my-saved-services/{serviceId}")]
        public async Task<IActionResult> SaveServiceToMyList(int serviceId)
        {
            var myPatientId = await GetCurrentPatientIdAsync();
            if (myPatientId == null)
            {
                return Unauthorized();
            }

            var success = await _patientManager.SaveServiceAsync(myPatientId.Value, serviceId);

            if (!success)
            {
                return NotFound("Patient or Service not found.");
            }
            // Return 204 for idempotent operation (works for both new and existing saves)
            return NoContent();
        }

        [HttpDelete("my-saved-services/{serviceId}")]
        public async Task<IActionResult> RemoveServiceFromMyList(int serviceId)
        {
            var myPatientId = await GetCurrentPatientIdAsync();
            if (myPatientId == null)
            {
                return Unauthorized();
            }

            var success = await _patientManager.RemoveSavedServiceAsync(myPatientId.Value, serviceId);

            if (!success)
            {
                return NotFound("Saved service link not found.");
            }
            return NoContent();
        }
    }
}