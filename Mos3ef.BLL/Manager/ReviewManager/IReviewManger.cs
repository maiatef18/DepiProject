using Mos3ef.BLL.Dtos.Review;
using Mos3ef.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mos3ef.BLL.Manager.ReviewManager
{
    /// <summary>
    /// Interface for review-related business operations.
    /// Methods throw exceptions for error cases instead of returning bool.
    /// </summary>
    public interface IReviewManger
    {
        Task<IEnumerable<ReviewReadDto>> GetReviewsByServiceIdAsync(int serviceId);
        
        /// <exception cref="Mos3ef.Api.Exceptions.BadRequestException">Thrown when ServiceId or PatientId invalid</exception>
        Task AddReviewAsync(ReviewAddDto review);
        
        /// <exception cref="Mos3ef.Api.Exceptions.NotFoundException">Thrown when review not found</exception>
        Task UpdateReviewAsync(ReviewUpdateDto review);
        
        /// <exception cref="Mos3ef.Api.Exceptions.NotFoundException">Thrown when review not found</exception>
        Task DeleteReviewAsync(int id);
    }
}

