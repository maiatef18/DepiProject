using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mos3ef.BLL.Dtos.Review;
using Mos3ef.BLL.Manager.ReviewManager;
using Mos3ef.DAL.Wapper;
using Mos3ef.Api.Exceptions;

namespace Mos3ef.Api.Controllers
{
    /// <summary>
    /// Controller for managing service reviews.
    /// 
    /// Following Clean Architecture:
    /// - Controller is thin - only handles HTTP concerns
    /// - Business logic and validation are in the Manager layer
    /// - Exceptions from Manager propagate to GlobalExceptionMiddleware
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewManger _reviewManger;

        public ReviewController(IReviewManger reviewManger)
        {
            _reviewManger = reviewManger;
        }

        /// <summary>
        /// Get all reviews for a specific service.
        /// </summary>
        [HttpGet("{serviceId}")]
        public async Task<IActionResult> GetReviewsByService(int serviceId)
        {
            if (serviceId <= 0)
                throw new BadRequestException("Service ID must be greater than 0.");

            var reviews = await _reviewManger.GetReviewsByServiceIdAsync(serviceId);
            return Ok(Response<IEnumerable<ReviewReadDto>>.Success(reviews, "Reviews retrieved successfully"));
        }

        /// <summary>
        /// Add a new review for a service.
        /// </summary>
        [Authorize(Policy = "Patient")]
        [HttpPost]
        public async Task<IActionResult> AddReview([FromBody] ReviewAddDto review)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                throw new ValidationException(errors);
            }

            // Manager throws BadRequestException if ServiceId or PatientId invalid
            await _reviewManger.AddReviewAsync(review);
            return Ok(Response<bool>.Success(true, "Review created successfully"));
        }

        /// <summary>
        /// Update an existing review.
        /// </summary>
        [Authorize(Policy = "Patient")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ReviewUpdateDto review)
        {
            if (id <= 0)
                throw new BadRequestException("Review ID must be greater than 0.");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                throw new ValidationException(errors);
            }

            // Manager throws NotFoundException if review not found
            await _reviewManger.UpdateReviewAsync(review);
            return Ok(Response<bool>.Success(true, "Review updated successfully"));
        }

        /// <summary>
        /// Delete a review.
        /// </summary>
        [Authorize(Policy = "Patient")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                throw new BadRequestException("Review ID must be greater than 0.");

            // Manager throws NotFoundException if review not found
            await _reviewManger.DeleteReviewAsync(id);
            return Ok(Response<bool>.Success(true, "Review deleted successfully"));
        }
    }
}

