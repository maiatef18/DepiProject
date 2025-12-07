using Microsoft.AspNetCore.Mvc;
using Mos3ef.BLL.Dtos.Compare;
using Mos3ef.BLL.Dtos.Common;
using Mos3ef.BLL.Dtos.Hospital;
using Mos3ef.BLL.Dtos.Review;
using Mos3ef.BLL.Dtos.Services;
using Mos3ef.BLL.Manager.ServiceManager;
using Mos3ef.DAL.Enum;
using Mos3ef.DAL.Wapper;
using Mos3ef.Api.Exceptions;

namespace Mos3ef.Api.Controllers
{
    /// <summary>
    /// Controller for searching, comparing, and retrieving medical services.
    /// 
    /// Following Clean Architecture:
    /// - Controller is thin - only handles HTTP concerns
    /// - Business logic and validation are in the Manager layer
    /// - Exceptions from Manager propagate to GlobalExceptionMiddleware
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public ServicesController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        /// <summary>
        /// Search services by keyword, category, and/or location.
        /// </summary>
        [HttpGet]
        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] string? keyword,
            [FromQuery] string? category,
            [FromQuery] double? lat,
            [FromQuery] double? lon)
        {
            CategoryType? catEnum = null;

            if (!string.IsNullOrWhiteSpace(category))
            {
                if (Enum.TryParse<CategoryType>(category, true, out var parsed))
                    catEnum = parsed;
            }

            var result = await _serviceManager.SearchServicesAsync(keyword, catEnum, lat, lon);
            return Ok(Response<IEnumerable<ServiceReadDto>>.Success(result, "Services retrieved successfully"));
        }

        /// <summary>
        /// Get service by ID with full details.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetServiceById(int id)
        {
            if (id <= 0)
                throw new BadRequestException("Service ID must be greater than 0.");

            // Manager throws NotFoundException if not found
            var service = await _serviceManager.GetByIdAsync(id);
            return Ok(Response<ServiceReadDto>.Success(service, "Service retrieved successfully."));
        }

        /// <summary>
        /// Compare two services side-by-side with detailed metrics.
        /// </summary>
        [HttpPost("compare")] 
        public async Task<IActionResult> CompareServices([FromBody] CompareRequestDto compareDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                throw new ValidationException(errors);
            }

            if (compareDto.Service1Id == compareDto.Service2Id)
                throw new BadRequestException("Cannot compare a service with itself.");

            // Manager throws NotFoundException if one or both services not found
            var result = await _serviceManager.CompareServicesAsync(compareDto);
            return Ok(Response<CompareResponseDto>.Success(result, "Services compared successfully."));
        }

        /// <summary>
        /// Get all reviews for a specific service.
        /// </summary>
        [HttpGet("{id}/reviews")]
        public async Task<IActionResult> GetServiceReviews(int id)
        {
            if (id <= 0)
                throw new BadRequestException("Service ID must be greater than 0.");

            // Verify service exists (throws if not found)
            await _serviceManager.GetByIdAsync(id);

            var reviews = await _serviceManager.GetServiceReviews(id);
            return Ok(Response<IEnumerable<ReviewReadDto>>.Success(reviews, "Reviews retrieved successfully."));
        }

        /// <summary>
        /// Get hospital information for a specific service.
        /// </summary>
        [HttpGet("{id}/hospital")]
        public async Task<IActionResult> GetServiceHospital(int id)
        {
            if (id <= 0)
                throw new BadRequestException("Service ID must be greater than 0.");

            // Verify service exists (throws if not found)
            await _serviceManager.GetByIdAsync(id);

            // Manager throws NotFoundException if hospital not found
            var hospital = await _serviceManager.GetServiceHospital(id);
            return Ok(Response<HospitalReadDto>.Success(hospital, "Hospital information retrieved successfully."));
        }
    }
}


