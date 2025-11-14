using Microsoft.AspNetCore.Mvc;
using Mos3ef.BLL.Dtos.Compare;
using Mos3ef.BLL.Dtos.Common;
using Mos3ef.BLL.Dtos.Hospital;
using Mos3ef.BLL.Dtos.Review;
using Mos3ef.BLL.Dtos.Services;
using Mos3ef.BLL.Manager.ServiceManager;

namespace Mos3ef.Api.Controllers
{
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
        /// Get all available services with pagination
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllServices(
            [FromQuery] bool onlyAvailable = true,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1)
            {
                return BadRequest(ApiResponse<PagedResult<ServiceReadDto>>.Failure("Page number must be at least 1."));
            }

            if (pageSize < 1 || pageSize > 100)
            {
                return BadRequest(ApiResponse<PagedResult<ServiceReadDto>>.Failure("Page size must be between 1 and 100."));
            }

            var filter = new ServiceFilterDto
            {
                OnlyAvailable = onlyAvailable,
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortBy = "name",
                IsAscending = true
            };

            var result = await _serviceManager.FilterServicesAsync(filter);
            return Ok(ApiResponse<PagedResult<ServiceReadDto>>.Success(result, "Services retrieved successfully."));
        }

        /// <summary>
        /// Get service by ID with full details including reviews and hospital information
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetServiceById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(ApiResponse<ServiceReadDto>.Failure("Service ID must be greater than 0."));
            }

            var service = await _serviceManager.GetByIdAsync(id);
            if (service == null)
            {
                return NotFound(ApiResponse<ServiceReadDto>.Failure($"Service with ID {id} not found."));
            }

            return Ok(ApiResponse<ServiceReadDto>.Success(service, "Service retrieved successfully."));
        }

        /// <summary>
        /// Search services by keyword with optional location-based filtering
        /// </summary>
        [HttpGet("search")] 
        public async Task<IActionResult> SearchServices(
            [FromQuery] string? keyword, 
            [FromQuery] double? lat, 
            [FromQuery] double? lng, 
            [FromQuery] double? radiusKm,
            [FromQuery] bool onlyAvailable = true,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1)
            {
                return BadRequest(ApiResponse<PagedResult<ServiceReadDto>>.Failure("Page number must be at least 1."));
            }

            if (pageSize < 1 || pageSize > 100)
            {
                return BadRequest(ApiResponse<PagedResult<ServiceReadDto>>.Failure("Page size must be between 1 and 100."));
            }

            var filter = new ServiceFilterDto
            {
                Keyword = keyword,
                OnlyAvailable = onlyAvailable,
                UserLatitude = lat,
                UserLongitude = lng,
                RadiusKm = radiusKm,
                SortBy = lat.HasValue && lng.HasValue ? "distance" : "name",
                IsAscending = true,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _serviceManager.FilterServicesAsync(filter);
            return Ok(ApiResponse<PagedResult<ServiceReadDto>>.Success(result, "Services retrieved successfully."));
        }

        /// <summary>
        /// Filter services with advanced filtering options
        /// </summary>
        [HttpGet("filter")]
        public async Task<IActionResult> FilterServices([FromQuery] ServiceFilterDto filterDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ApiResponse<PagedResult<ServiceReadDto>>.Failure("Validation failed.", errors));
            }

            var result = await _serviceManager.FilterServicesAsync(filterDto);
            return Ok(ApiResponse<PagedResult<ServiceReadDto>>.Success(result, "Services filtered successfully."));
        }

        /// <summary>
        /// Compare two services side-by-side with detailed metrics
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
                return BadRequest(ApiResponse<CompareResponseDto>.Failure("Validation failed.", errors));
            }

            if (compareDto.Service1Id == compareDto.Service2Id)
            {
                return BadRequest(ApiResponse<CompareResponseDto>.Failure("Cannot compare a service with itself."));
            }

            var result = await _serviceManager.CompareServicesAsync(compareDto);
            
            if (result.Service1 == null || result.Service2 == null)
            {
                return NotFound(ApiResponse<CompareResponseDto>.Failure("One or both services not found."));
            }

            return Ok(ApiResponse<CompareResponseDto>.Success(result, "Services compared successfully."));
        }

        /// <summary>
        /// Get reviews for a specific service
        /// </summary>
        [HttpGet("{id}/reviews")]
        public async Task<IActionResult> GetServiceReviews(int id)
        {
            if (id <= 0)
            {
                return BadRequest(ApiResponse<IEnumerable<ReviewReadDto>>.Failure("Service ID must be greater than 0."));
            }

            // Verify service exists
            var service = await _serviceManager.GetByIdAsync(id);
            if (service == null)
            {
                return NotFound(ApiResponse<IEnumerable<ReviewReadDto>>.Failure($"Service with ID {id} not found."));
            }

            var reviews = await _serviceManager.GetServiceReviews(id);
            return Ok(ApiResponse<IEnumerable<ReviewReadDto>>.Success(reviews, "Reviews retrieved successfully."));
        }

        /// <summary>
        /// Get hospital information for a specific service
        /// </summary>
        [HttpGet("{id}/hospital")]
        public async Task<IActionResult> GetServiceHospital(int id)
        {
            if (id <= 0)
            {
                return BadRequest(ApiResponse<HospitalReadDto>.Failure("Service ID must be greater than 0."));
            }

            // Verify service exists
            var service = await _serviceManager.GetByIdAsync(id);
            if (service == null)
            {
                return NotFound(ApiResponse<HospitalReadDto>.Failure($"Service with ID {id} not found."));
            }

            var hospital = await _serviceManager.GetServiceHospital(id);
            if (hospital == null)
            {
                return NotFound(ApiResponse<HospitalReadDto>.Failure($"Hospital not found for service with ID {id}."));
            }

            return Ok(ApiResponse<HospitalReadDto>.Success(hospital, "Hospital information retrieved successfully."));
        }
    }
}
