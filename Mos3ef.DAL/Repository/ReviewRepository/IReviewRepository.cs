using Mos3ef.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mos3ef.DAL.Repository.ReviewRepository
{
    public interface IReviewRepository
    {
        IQueryable<Review> GetReviewsByServiceId(int serviceId);
        
        Review GetReviewById(int reviewId);
        void AddReview(Review review);
        void UpdateReview(Review review);
        void DeleteReview(Review review);

    }
}
