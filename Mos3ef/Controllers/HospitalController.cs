using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mos3ef.BLL.Dtos.Hospital;
using Mos3ef.BLL.Dtos.Services;
using Mos3ef.BLL.Manager;
using Mos3ef.DAL.Database;

namespace Mos3ef.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HospitalController : ControllerBase
    {

        private readonly IHospitalManager _hospitalManager;

        public HospitalController(IHospitalManager hospitalManager)
        {
            _hospitalManager = hospitalManager;
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
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] int id, [FromBody] HospitalUpdateDto hospitalUpdateDto)
        {
            if (id != hospitalUpdateDto.Id)
                return BadRequest("Mismatched Hospital ID");

            await _hospitalManager.UpdateAsync(hospitalUpdateDto);
            return NoContent();
        }


        [Authorize(Policy = "Hospital")]
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        {
            await _hospitalManager.DeleteAsync(id);
            return NoContent();
        }


        [Authorize(Policy = "Hospital")]
        [HttpPost("AddService")]
        public async Task<IActionResult> AddServiceAsync([FromBody] ServicesAddDto servicesAddDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            int id = await _hospitalManager.AddServiceAsync(servicesAddDto);
            return Ok(new { ServiceId = id });
        }


        [Authorize(Policy = "Hospital")]
        [HttpPut("UpdateService/{id}")]
        public async Task<IActionResult> UpdateServiceAsync([FromRoute] int id, [FromBody] ServicesUpdateDto servicesUpdateDto)
        {
            if (id != servicesUpdateDto.ServiceId)
                return BadRequest("Mismatched Service ID");

            await _hospitalManager.UpdateServiceAsync(servicesUpdateDto);
            return NoContent();
        }


        [Authorize(Policy = "Hospital")]
        [HttpDelete("DeleteService/{id}")]
        public async Task<IActionResult> DeleteServiceAsync([FromRoute] int id)
        {
            await _hospitalManager.DeleteServiceAsync(id);
            return NoContent();
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