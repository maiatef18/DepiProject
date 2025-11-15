using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mos3ef.BLL.Dtos.Review;
using Mos3ef.BLL.Manager.ReviewManager;
using Mos3ef.DAL.Wapper;

namespace Mos3ef.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewManger _reviewManger;

        public ReviewController(IReviewManger reviewManger)
        {
            _reviewManger = reviewManger;
        }
        [HttpGet("{serviceId}")]
        public async Task<IActionResult> GetReviewsByService(int serviceId)
        {
            var reviews = await _reviewManger.GetReviewsByServiceIdAsync(serviceId);
            return Ok(new Response<IEnumerable<ReviewReadDto>>(reviews, "reviews return Successfully"));
        }

        [Authorize(Policy = "Patient")]
        [HttpPost]
        public async Task<IActionResult> AddReview(ReviewAddDto review)
        {
              
          bool result = await _reviewManger.AddReviewAsync(review);
            if (!result)
                return BadRequest(new Response<bool>("Error in ServiceId or PatientId"));

            return Ok(new Response<bool>(true, "Review Created Successfully"));
        }

        [Authorize(Policy = "Patient")]
        [HttpPut("{Id}")]
        public async Task<IActionResult> Update(int Id,ReviewUpdateDto review)
        {

         bool result=  await _reviewManger.UpdateReviewAsync(review);
            if (!result)
                return NotFound(new Response<bool>("Review not found"));

            return Ok(new Response<bool>(true, "Review Updated Successfully"));

        }

        [Authorize(Policy = "Patient")]
        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(int Id)
        {
            var isDeleted = await _reviewManger.DeleteReviewAsync(Id);

            if (!isDeleted)
                return NotFound(new Response<bool>("Review not found"));

            return Ok(new Response<bool>(true, "Review Deleted Successfully"));
        }
    }
}
