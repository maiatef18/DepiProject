using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mos3ef.BLL.Dtos.Hospital;
using Mos3ef.BLL.Dtos.Services;
using Mos3ef.BLL.Manager.HospitalManager;
using Mos3ef.DAL.Database;
using System.Security.Claims;

namespace Mos3ef.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HospitalController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly IHospitalManager _hospitalManager;

        public HospitalController(IHospitalManager hospitalManager, IWebHostEnvironment env)
        {
            _hospitalManager = hospitalManager;
            _env = env;
        }


        [Authorize(Policy = "Patient")]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var hospitals = await Task.Run(() => _hospitalManager.GetAllAsync());
            return Ok(hospitals);
        }


        [Authorize(Policy = "Patient")]
        [HttpGet("Get/{id}")]
        public async Task<IActionResult> GetAsync([FromRoute] int id)
        {
            var hospital = await _hospitalManager.GetAsync(id);
            if (hospital == null)
                return NotFound($"Hospital with ID {id} not found.");

            return Ok(hospital);
        }


        [Authorize(Policy = "Hospital")]
        [HttpPost("Add")]
        public async Task<IActionResult> AddAsync([FromBody] HospitalAddDto hospitalAddDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            int id = await _hospitalManager.AddAsync(hospitalAddDto);
            return Ok(new { HospitalId = id });
        }


        [Authorize(Policy = "Hospital")]
        [HttpPut("Update/Id")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateAsync(HospitalUpdateDto hospitalUpdateDto)
        {
            var Hospital_ID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _hospitalManager.UpdateAsync(Hospital_ID, hospitalUpdateDto);
            return NoContent(); 
        }


        [Authorize(Policy = "Hospital")]
        [HttpDelete("Delete/Id")]
        public async Task<IActionResult> DeleteAsync()
        {
            var Hospital_ID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _hospitalManager.DeleteAsync(Hospital_ID);
            return NoContent();
        }


        [Authorize(Policy = "Hospital")]
        [HttpPost("AddService")]
        public async Task<IActionResult> AddServiceAsync([FromBody] ServicesAddDto servicesAddDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            int id = await _hospitalManager.AddServiceAsync(userId , servicesAddDto);
            return Ok(new { ServiceId = id });
        }


        [Authorize(Policy = "Hospital")]
        [HttpPut("UpdateService/{id}")]
        public async Task<IActionResult> UpdateServiceAsync([FromRoute] int id, [FromBody] ServicesUpdateDto servicesUpdateDto)
        {
            var Hospital_Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            await _hospitalManager.UpdateServiceAsync(Hospital_Id, id, servicesUpdateDto);
            return Ok("Updated");
        }

        [Authorize(Policy = "Hospital")]
        [HttpDelete("DeleteService/{id}")]
        public async Task<IActionResult> DeleteServiceAsync([FromRoute] int id)
        {
            var Hospital_Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _hospitalManager.DeleteServiceAsync(Hospital_Id, id);
            return Ok("Deleted");
        }


        [Authorize(Policy = "Hospital")]
        [HttpGet("GetServicesReviews/{id}")]
        public async Task<IActionResult> GetServicesReviewsAsync([FromRoute] int id)
        {
            var reviews = await _hospitalManager.GetServicesReviewsAsync(id);
            return Ok(reviews);
        }


        [Authorize(Policy = "Hospital")]
        [HttpGet("GetDashboardStats/{id}")]
        public async Task<IActionResult> GetDashboardStatsAsync([FromRoute] int id)
        {
            var stats = await _hospitalManager.GetDashboardStatsAsync(id);
            return Ok(stats);
        }


    }
}