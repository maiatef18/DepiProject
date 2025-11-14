using Microsoft.AspNetCore.Mvc;
using Mos3ef.BLL.Dtos.Patient;
using Mos3ef.BLL.Dtos.Common;
using Mos3ef.BLL.Manager.PatientManager;
using Mos3ef.DAL.Models;
using System.Security.Claims;
using Mos3ef.BLL.Dtos.Services;
using Microsoft.AspNetCore.Authorization;

namespace Mos3ef.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class PatientsController : ControllerBase
    {
        private readonly IPatientManager _patientManager;

        public PatientsController(IPatientManager patientManager)
        {
            _patientManager = patientManager;
        }

        private async Task<int?> GetCurrentPatientIdAsync()
        {
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

            return patient.PatientId;
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

        [HttpGet("GetMyProfile")]
        public async Task<ActionResult<PatientReadDto>> GetMyProfile()
        {
            var patientId = await GetCurrentPatientIdAsync();
            if (patientId == null) return Unauthorized();

            var patient = await _patientManager.GetPatientByIdAsync(patientId.Value);
            if (patient == null) return NotFound();

            return Ok(patient);
        }

        [HttpPut("UpdateMyProfile")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] PatientUpdateDto patientUpdateDto)
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

            var success = await _patientManager.UpdatePatientAsync(myPatientId.Value, patientUpdateDto);
            if (!success)
            {
                return NotFound("Patient profile not found.");
            }
            return NoContent();
        }

        [HttpGet("GetMySavedServices")]
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

        [HttpPost("SaveServiceToMyList/{serviceId}")]
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
            return StatusCode(201, "Service saved successfully.");
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