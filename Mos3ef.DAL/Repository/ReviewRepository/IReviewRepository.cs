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
       Task<IEnumerable<Review>> GetReviewsByServiceIdAsync(int serviceId);
        
        Task<Review> GetReviewByIdAsync(int reviewId);
        Task<int> AddReviewAsync(Review review);
        Task<int> UpdateReviewAsync(Review review);
        Task DeleteReviewAsync(Review review);

    }
}
