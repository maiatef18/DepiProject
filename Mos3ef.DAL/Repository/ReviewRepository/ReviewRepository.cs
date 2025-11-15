using Microsoft.EntityFrameworkCore;
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
        

        public async Task<IEnumerable<Review>> GetReviewsByServiceIdAsync(int serviceId)
        {
            var reviews =await _applicationDbContext.Reviews
                .Where(r => r.ServiceId == serviceId).ToListAsync();
            return reviews;
        }

        public async Task<Review?> GetReviewByIdAsync(int reviewId)
        {
            var review = await _applicationDbContext.Reviews
                .FirstOrDefaultAsync(r => r.ReviewId == reviewId);
            return review;
      
        }

        public async Task<bool> AddReviewAsync(Review review)
        {
            // Check if Service exists
            bool serviceExists = await _applicationDbContext.Services
                .AnyAsync(s => s.ServiceId == review.ServiceId);
            if (!serviceExists)
                return false;

            // Check if Patient exists
            bool patientExists = await _applicationDbContext.Patients
                .AnyAsync(p => p.PatientId == review.PatientId);
            if (!patientExists)
                return false;

            _applicationDbContext.Reviews.Add(review);
            await _applicationDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<int> UpdateReviewAsync(Review review)
        {
            _applicationDbContext.Reviews.Update(review);
           await _applicationDbContext.SaveChangesAsync();
            return review.ReviewId;
        }

        public async Task<bool> DeleteReviewAsync(Review review)
        {
            if (review == null)
                return false;

            _applicationDbContext.Reviews.Remove(review);
            await _applicationDbContext.SaveChangesAsync();
            return true;
        }
    }
}
