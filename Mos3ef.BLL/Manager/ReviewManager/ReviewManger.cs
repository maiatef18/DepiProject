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
using Mos3ef.Api.Exceptions;

namespace Mos3ef.BLL.Manager.ReviewManager
{
    /// <summary>
    /// Manager for review-related business logic.
    /// Throws business exceptions for error cases.
    /// </summary>
    public class ReviewManger : IReviewManger
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;

        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);

        public ReviewManger(IReviewRepository reviewRepository, IMapper mapper, IMemoryCache memoryCache)
        {
            _reviewRepository = reviewRepository;
            _mapper = mapper;
            _memoryCache = memoryCache;
        }

        public async Task<IEnumerable<ReviewReadDto>> GetReviewsByServiceIdAsync(int serviceId)
        {
            string cacheKey = $"{CacheConstant.reviewCacheKey}_{serviceId}";

            if (!_memoryCache.TryGetValue(cacheKey, out IEnumerable<ReviewReadDto> cachedReviews))
            {
                var reviews = await _reviewRepository.GetReviewsByServiceIdAsync(serviceId);
                var reviewsResult = _mapper.Map<IEnumerable<ReviewReadDto>>(reviews).ToList();

                _memoryCache.Set(cacheKey, reviewsResult, CacheDuration);
                cachedReviews = reviewsResult;
            }

            return cachedReviews;
        }

        /// <exception cref="BadRequestException">Thrown when ServiceId or PatientId is invalid</exception>
        public async Task AddReviewAsync(ReviewAddDto review)
        {
            var newReview = _mapper.Map<Review>(review);
            newReview.Review_Date = DateTime.Now;
            
            bool result = await _reviewRepository.AddReviewAsync(newReview);
            if (!result)
                throw new BadRequestException("Invalid ServiceId or PatientId.");

            string cacheKey = $"{CacheConstant.reviewCacheKey}_{review.ServiceId}";
            _memoryCache.Remove(cacheKey);
        }

        /// <exception cref="NotFoundException">Thrown when review not found</exception>
        public async Task UpdateReviewAsync(ReviewUpdateDto review)
        {
            var existingReview = await _reviewRepository.GetReviewByIdAsync(review.ReviewId);
            if (existingReview == null)
                throw new NotFoundException($"Review with ID {review.ReviewId} not found.");

            await _reviewRepository.UpdateReviewAsync(_mapper.Map<ReviewUpdateDto, Review>(review, existingReview));

            string cacheKey = $"{CacheConstant.reviewCacheKey}_{existingReview.ServiceId}";

            // Update cache after updating review
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
        }

        /// <exception cref="NotFoundException">Thrown when review not found</exception>
        public async Task DeleteReviewAsync(int id)
        {
            var reviewToDelete = await _reviewRepository.GetReviewByIdAsync(id);
            if (reviewToDelete == null)
                throw new NotFoundException($"Review with ID {id} not found.");

            await _reviewRepository.DeleteReviewAsync(reviewToDelete);

            string cacheKey = $"{CacheConstant.reviewCacheKey}_{reviewToDelete.ServiceId}";

            // Update cache
            if (_memoryCache.TryGetValue(cacheKey, out List<ReviewReadDto> reviewsDtos))
            {
                var updatedReviews = reviewsDtos.Where(r => r.ReviewId != id).ToList();
                _memoryCache.Set(cacheKey, updatedReviews, CacheDuration);
            }
        }
    }
}

