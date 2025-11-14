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
           await _reviewManger.AddReviewAsync(review);
            return Ok(new Response<bool>(true, "Review Created Successfully"));
        }

        [Authorize(Policy = "Patient")]
        [HttpPut]
        public async Task<IActionResult> Update(int Id, ReviewUpdateDto review)
        {
            if (Id != review.ReviewId)
            {
                return BadRequest(new Response<bool>(false, "Error in Id"));
            }

           await _reviewManger.UpdateReviewAsync(review);
            return Ok(new Response<bool>(true, "Review Updated Successfully"));

        }

        [Authorize(Policy = "Patient")]
        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(int Id)
        {
           await _reviewManger.DeleteReviewAsync(Id);
            return Ok(new Response<bool>(true, "Review Deleted Successfully"));

        }
    }
}
