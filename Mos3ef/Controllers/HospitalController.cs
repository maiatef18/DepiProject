using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mos3ef.BLL.Dtos.Hospital;
using Mos3ef.BLL.Dtos.Review;
using Mos3ef.BLL.Dtos.Services;
using Mos3ef.BLL.Manager.HospitalManager;
using Mos3ef.DAL.Database;
using Mos3ef.DAL.Models;
using Mos3ef.DAL.Wapper;
using System.Security.Claims;
using Mos3ef.Api.Exceptions;

namespace Mos3ef.Api.Controllers
{
    /// <summary>
    /// Controller for hospital management operations.
    /// 
    /// Following Clean Architecture:
    /// - Controller is thin - only handles HTTP concerns
    /// - Business logic and validation are in the Manager layer
    /// - Exceptions from Manager propagate to GlobalExceptionMiddleware
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class HospitalController : ControllerBase
    {
        private readonly IHospitalManager _hospitalManager;

        public HospitalController(IHospitalManager hospitalManager)
        {
            _hospitalManager = hospitalManager;
        }

        /// <summary>
        /// Get all hospitals (for patients to browse).
        /// </summary>
        [Authorize(Policy = "Patient")]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var hospitals = await _hospitalManager.GetAllAsync();
            return Ok(Response<IEnumerable<HospitalReadDto>>.Success(hospitals, "Hospitals fetched"));
        }

        /// <summary>
        /// Get the logged-in hospital's profile.
        /// </summary>
        [Authorize(Policy = "Hospital")]
        [HttpGet("Get-Profile")]
        public async Task<IActionResult> GetAsync()
        {
            var hospitalId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(hospitalId))
                throw new UnauthorizedException("User not authenticated.");

            // Manager throws NotFoundException if not found
            var hospital = await _hospitalManager.GetAsync(hospitalId);
            return Ok(Response<HospitalReadDto>.Success(hospital, "Hospital fetched"));
        }

        /// <summary>
        /// Add a new hospital.
        /// </summary>
        [Authorize(Policy = "Hospital")]
        [HttpPost("Add")]
        public async Task<IActionResult> AddAsync([FromBody] HospitalAddDto hospitalAddDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                throw new ValidationException(errors);
            }

            var hospital = await _hospitalManager.AddAsync(hospitalAddDto);
            return Ok(Response<Hospital>.Success(hospital, "Hospital added successfully"));
        }

        /// <summary>
        /// Update the logged-in hospital's profile.
        /// </summary>
        [Authorize(Policy = "Hospital")]
        [HttpPut("Update-profile")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateAsync([FromForm] HospitalUpdateDto hospitalUpdateDto)
        {
            var hospitalId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(hospitalId))
                throw new UnauthorizedException("User not authenticated.");

            // Manager throws NotFoundException if not found
            var hospital = await _hospitalManager.UpdateAsync(hospitalId, hospitalUpdateDto);
            return Ok(Response<HospitalReadDto>.Success(hospital, "Hospital updated successfully"));
        }

        /// <summary>
        /// Delete the logged-in hospital's profile.
        /// </summary>
        [Authorize(Policy = "Hospital")]
        [HttpDelete("Delete-Profile")]
        public async Task<IActionResult> DeleteAsync()
        {
            var hospitalId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(hospitalId))
                throw new UnauthorizedException("User not authenticated.");

            // Manager throws NotFoundException if not found
            await _hospitalManager.DeleteAsync(hospitalId);
            return Ok(Response<bool>.Success(true, "Hospital deleted successfully"));
        }

        /// <summary>
        /// Add a new service to the hospital.
        /// </summary>
        [Authorize(Policy = "Hospital")]
        [HttpPost("AddService")]
        public async Task<IActionResult> AddServiceAsync([FromBody] ServicesAddDto servicesAddDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedException("User not authenticated.");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                throw new ValidationException(errors);
            }

            // Manager throws NotFoundException if hospital not registered
            var service = await _hospitalManager.AddServiceAsync(userId, servicesAddDto);
            return Ok(Response<ServiceShowDto>.Success(service, "Service added successfully"));
        }

        /// <summary>
        /// Get all services for the logged-in hospital.
        /// </summary>
        [Authorize(Policy = "Hospital")]
        [HttpGet("GetAllServices")]
        public async Task<IActionResult> GetAllServices()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedException("User not authenticated.");

            var services = await _hospitalManager.GetAllServicesAsync(userId);
            return Ok(Response<IEnumerable<ServiceHospitalDto>>.Success(services, "Services fetched"));
        }

        /// <summary>
        /// Update a specific service.
        /// </summary>
        [Authorize(Policy = "Hospital")]
        [HttpPut("UpdateService/{id}")]
        public async Task<IActionResult> UpdateServiceAsync([FromRoute] int id, [FromBody] ServicesUpdateDto servicesUpdateDto)
        {
            if (id <= 0)
                throw new BadRequestException("Service ID must be greater than 0.");

            var hospitalId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(hospitalId))
                throw new UnauthorizedException("User not authenticated.");

            // Manager throws NotFoundException if not found
            var service = await _hospitalManager.UpdateServiceAsync(hospitalId, id, servicesUpdateDto);
            return Ok(Response<ServiceShowDto>.Success(service, "Service updated successfully"));
        }

        /// <summary>
        /// Delete a specific service.
        /// </summary>
        [Authorize(Policy = "Hospital")]
        [HttpDelete("DeleteService/{id}")]
        public async Task<IActionResult> DeleteServiceAsync([FromRoute] int id)
        {
            if (id <= 0)
                throw new BadRequestException("Service ID must be greater than 0.");

            var hospitalId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(hospitalId))
                throw new UnauthorizedException("User not authenticated.");

            // Manager throws NotFoundException if not found
            await _hospitalManager.DeleteServiceAsync(hospitalId, id);
            return Ok(Response<bool>.Success(true, "Service deleted successfully"));
        }

        /// <summary>
        /// Get all reviews for a hospital's services.
        /// </summary>
        [Authorize(Policy = "Hospital")]
        [HttpGet("GetServicesReviews/{id}")]
        public async Task<IActionResult> GetServicesReviewsAsync([FromRoute] int id)
        {
            if (id <= 0)
                throw new BadRequestException("Hospital ID must be greater than 0.");

            var reviews = await _hospitalManager.GetServicesReviewsAsync(id);
            return Ok(Response<IEnumerable<ReviewReadDto>>.Success(reviews, "Reviews fetched"));
        }

        /// <summary>
        /// Get dashboard statistics for a hospital.
        /// </summary>
        [Authorize(Policy = "Hospital")]
        [HttpGet("GetDashboardStats/{id}")]
        public async Task<IActionResult> GetDashboardStatsAsync([FromRoute] int id)
        {
            if (id <= 0)
                throw new BadRequestException("Hospital ID must be greater than 0.");

            var stats = await _hospitalManager.GetDashboardStatsAsync(id);
            return Ok(Response<(int servicesCount, int reviewsCount, double avgRating)>.Success(stats, "Stats fetched"));
        }
    }
}
