using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Mos3ef.BLL.Dtos.Review;

using Mos3ef.DAL;
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
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;

        // chache duration
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);

        public ReviewManger(IReviewRepository reviewRepository,IMapper mapper, IMemoryCache memoryCache)
        {
            _reviewRepository = reviewRepository;
            _mapper = mapper;
           _memoryCache = memoryCache;
        }
   

        public async Task<IEnumerable<ReviewReadDto>> GetReviewsByServiceIdAsync(int serviceId)
        {
            // chache key for service reviews
            string cacheKey = $"{CacheConstant.reviewCacheKey}_{serviceId}";

            // the cache check 
            if (!_memoryCache.TryGetValue(cacheKey, out IEnumerable<ReviewReadDto> cachedReviews))
            {
                var reviews = await _reviewRepository.GetReviewsByServiceIdAsync(serviceId);
                var reviewsResualt = _mapper.Map<IEnumerable<ReviewReadDto>>(reviews).ToList();

                _memoryCache.Set(cacheKey, reviewsResualt, CacheDuration);
                cachedReviews = reviewsResualt;
            }

            return cachedReviews;
        }

        public async Task<bool> AddReviewAsync(ReviewAddDto review)
        {
            var newReview = _mapper.Map<Review>(review);
            newReview.Review_Date = DateTime.Now;
          bool result= await _reviewRepository.AddReviewAsync(newReview);
            string cacheKey = $"{CacheConstant.reviewCacheKey}_{review.ServiceId}";
            _memoryCache.Remove(cacheKey);
            return result;
        }

        public async Task<bool> UpdateReviewAsync(ReviewUpdateDto review)
        {
            var existingReview = await _reviewRepository.GetReviewByIdAsync(review.ReviewId);
            if (existingReview == null)
                return false;

            await _reviewRepository.UpdateReviewAsync(_mapper.Map<ReviewUpdateDto, Review>(review, existingReview));

            string cacheKey = $"{CacheConstant.reviewCacheKey}_{existingReview.ServiceId}";

            // UPdate cache after updating review
            if (_memoryCache.TryGetValue(cacheKey, out List<ReviewReadDto> cachedReviews))
            {
                var reviewToUpdate = cachedReviews.FirstOrDefault(r => r.ReviewId == existingReview.ReviewId);
                if (reviewToUpdate != null)
                {
                    reviewToUpdate.Rating = existingReview.Rating;
                    reviewToUpdate.Review_Date = existingReview.Review_Date;
                    reviewToUpdate.Comment = existingReview.Comment;
                }

                _memoryCache.Set(cacheKey, cachedReviews, CacheDuration);
            }
                return true;
        }

        public async Task<bool> DeleteReviewAsync(int Id)
        {
            var reviewToDelete = await _reviewRepository.GetReviewByIdAsync(Id);

            if (reviewToDelete == null)
                return false;

            await _reviewRepository.DeleteReviewAsync(reviewToDelete);

            string cacheKey = $"{CacheConstant.reviewCacheKey}_{reviewToDelete.ServiceId}";

            // Update cache
            if (_memoryCache.TryGetValue(cacheKey, out List<ReviewReadDto> reviewsDtos))
            {
                var updatedReviews = reviewsDtos.Where(r => r.ReviewId != Id).ToList();
                _memoryCache.Set(cacheKey, updatedReviews, CacheDuration);
            }

            return true;
        }

    }
}
