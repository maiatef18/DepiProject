using Mos3ef.BLL.Dtos.ReviewDto;
using Mos3ef.DAL.Models;
using Mos3ef.DAL.Repository.ReviewRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mos3ef.BLL.Manager.ReviewManager
{
    public class ReviewManger : IReviewManger
    {
        private readonly IReviewRepository _reviewRepository;

        public ReviewManger(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }


        public IEnumerable<ReviewReadDto> GetReviewsByServiceId(int serviceId)
        {
            var reviews = _reviewRepository.GetReviewsByServiceId(serviceId);
            // Mapping Review to ReviewReadDto
            var reviewDtos = reviews.Select(r => new ReviewReadDto
            {
                ReviewId = r.ReviewId,
                Rating = r.Rating,
                Review_Date = r.Review_Date,
                Comment = r.Comment,
                ServiceId = r.ServiceId,
                PatientId = r.PatientId
            }).ToList();
            return reviewDtos;
        }
        public void AddReview(ReviewAddDto review)
        {
            // Mapping ReviewAddDto to Review
            //No tracking entity is being added here
            // Review_Date is set to current date
            var newReview = new Review
            {
                Rating = review.Rating,
                Review_Date = DateTime.Now,
                Comment = review.Comment,
                ServiceId = review.ServiceId,
                PatientId = review.PatientId
            };
            _reviewRepository.AddReview(newReview);
        }

        public void DeleteReview(int Id)
        {
            // Retrieve the review to be deleted
            var reviewToDelete = _reviewRepository.GetReviewById(Id);

            _reviewRepository.DeleteReview(reviewToDelete);
        }
        public void UpdateReview(ReviewUpdateDto review)
        {
            var existingReview = _reviewRepository.GetReviewById(review.ReviewId);

            // Mapping ReviewUpdateDto to Review
             existingReview.Rating = review.Rating;
             existingReview.Review_Date = review.Review_Date;
             existingReview.Comment = review.Comment;
             existingReview.ServiceId = review.ServiceId;
             existingReview.PatientId = review.PatientId;

            _reviewRepository.UpdateReview(existingReview);
        }
    }
}
