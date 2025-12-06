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
            var hospitals = await _hospitalManager.GetAllAsync();

            if (hospitals == null || !hospitals.Any())
                return Ok(Response<HospitalReadDto>.Fail("Hospital not found"));

            return Ok(Response<IEnumerable<HospitalReadDto>>.Success(hospitals, "Hospitals fetched"));
        }


        [Authorize(Policy = "Hospital")]
        [HttpGet("Get-Profile")]
        public async Task<IActionResult> GetAsync()
        {
            var Hospital_ID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var hospital = await _hospitalManager.GetAsync(Hospital_ID);
            if (hospital == null)
                return Ok( Response<Hospital>.Fail("No Hospital found"));

            return Ok(Response<HospitalReadDto>.Success(hospital, "Hospital fetched"));
        }


        [Authorize(Policy = "Hospital")]
        [HttpPost("Add")]
        public async Task<IActionResult> AddAsync([FromBody] HospitalAddDto hospitalAddDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var Hospital = await _hospitalManager.AddAsync(hospitalAddDto);
            return Ok(Response<Hospital>.Success(Hospital, "Hospital Added Successfully"));
        }


        [Authorize(Policy = "Hospital")]
        [HttpPut("Update-profile")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateAsync([FromForm]  HospitalUpdateDto hospitalUpdateDto)
        {
            var Hospital_ID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var Hospital = await _hospitalManager.UpdateAsync(Hospital_ID, hospitalUpdateDto);
            return Ok(( Response<HospitalReadDto>.Success(Hospital, "Hospital Updated Successfully")));
        }


        [Authorize(Policy = "Hospital")]
        [HttpDelete("Delete-Profile")]
        public async Task<IActionResult> DeleteAsync()
        {
            var Hospital_ID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _hospitalManager.DeleteAsync(Hospital_ID);
            return Ok(new Response<Hospital>("Hospital Deleted Successfully"));
        }


        [Authorize(Policy = "Hospital")]
        [HttpPost("AddService")]
        public async Task<IActionResult> AddServiceAsync([FromBody] ServicesAddDto servicesAddDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var Service = await _hospitalManager.AddServiceAsync(userId , servicesAddDto);
            return Ok( Response<ServiceShowDto>.Success(Service, "Service Added Successfully"));
        }

        [Authorize(Policy = "Hospital")]
        [HttpPost("GetAllServices")]
        public async Task<IActionResult> GetAllServices()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not found.");

            var services = await _hospitalManager.GetAllServicesAsync(userId);

            if (services == null || !services.Any())
                return Ok( Response<List<Service>>.Fail("No services found"));

            return Ok(Response<IEnumerable<ServiceHospitalDto>>.Success(services, "Services fetched"));
        }

        [Authorize(Policy = "Hospital")]
        [HttpPut("UpdateService/{id}")]
        public async Task<IActionResult> UpdateServiceAsync([FromRoute] int id, [FromBody] ServicesUpdateDto servicesUpdateDto)
        {
            var Hospital_Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var Service = await _hospitalManager.UpdateServiceAsync(Hospital_Id, id, servicesUpdateDto);
            return Ok( Response<ServiceShowDto>.Success(Service ,"Service Added Successfully"));
        }

        [Authorize(Policy = "Hospital")]
        [HttpDelete("DeleteService/{id}")]
        public async Task<IActionResult> DeleteServiceAsync([FromRoute] int id)
        {
            var Hospital_Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _hospitalManager.DeleteServiceAsync(Hospital_Id, id);
            return Ok(new Response<Service>("Service Deleted Successfully"));
        }


        [Authorize(Policy = "Hospital")]
        [HttpGet("GetServicesReviews/{id}")]
        public async Task<IActionResult> GetServicesReviewsAsync([FromRoute] int id)
        {
            var reviews = await _hospitalManager.GetServicesReviewsAsync(id);
            if (reviews == null)
            {
                return Ok( Response<Review>.Fail("Service not have Review "));

            }
            return Ok(Response<IEnumerable<ReviewReadDto>>.Success(reviews, "Reviews fetched"));
        }


        [Authorize(Policy = "Hospital")]
        [HttpGet("GetDashboardStats/{id}")]
        public async Task<IActionResult> GetDashboardStatsAsync([FromRoute] int id)
        {
            var stats = await _hospitalManager.GetDashboardStatsAsync(id);
            return Ok(Response<(int servicesCount, int reviewsCount, double avgRating)>.Success(stats, "Stats fetched"));
        }


    }
}