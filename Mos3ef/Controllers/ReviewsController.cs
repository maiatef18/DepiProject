using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mos3ef.BLL.Dtos.ReviewDto;
using Mos3ef.BLL.Manager.ReviewManager;

namespace Mos3ef.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewManger _reviewManger;

        public ReviewsController(IReviewManger reviewManger)
        {
            _reviewManger = reviewManger;
        }
        [HttpGet("{serviceId}")]
        public IActionResult GetReviewsByService(int serviceId)
        {
            var reviews = _reviewManger.GetReviewsByServiceId(serviceId);
            return Ok(reviews);
        }

        [HttpPost]
        public IActionResult Insert(ReviewAddDto review)
        {
            _reviewManger.AddReview(review);
            return CreatedAtAction(nameof(GetReviewsByService), new { serviceId = review.ServiceId }, new { Message = "Created Successfully" });
        }

        [HttpPut]
        public IActionResult Update(int Id,ReviewUpdateDto review)
        {
            if (Id != review.ReviewId)
            {
                return BadRequest();
            }

            _reviewManger.UpdateReview(review);
            return NoContent();
        }
        [HttpDelete("{Id}")]
        public IActionResult Delete(int Id)
        {
            _reviewManger.DeleteReview(Id);
            return NoContent();
        }
    }
}
