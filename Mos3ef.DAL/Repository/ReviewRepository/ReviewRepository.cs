using Mos3ef.DAL.Database;
using Mos3ef.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mos3ef.DAL.Repository.ReviewRepository
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public ReviewRepository(ApplicationDbContext applicationDbContext) 
        {
            _applicationDbContext = applicationDbContext;
        }
        
        public IQueryable<Review> GetReviewsByServiceId(int serviceId)
        {

            var reviews = _applicationDbContext.Reviews
                .Where(r => r.ServiceId == serviceId);
                
            return reviews;

        }

        public Review GetReviewById(int reviewId)
        {
            var review = _applicationDbContext.Reviews.Find(reviewId);
            return review;

        }
        public void AddReview(Review review)
        {
            _applicationDbContext.Reviews.Add(review);
            _applicationDbContext.SaveChanges();
        }

        public void DeleteReview(Review review)
        {
           _applicationDbContext.Reviews.Remove(review);
              _applicationDbContext.SaveChanges();
        }
        public void UpdateReview(Review review)
        {
            _applicationDbContext.Reviews.Update(review);
            _applicationDbContext.SaveChanges();
        }

       
    }
}
