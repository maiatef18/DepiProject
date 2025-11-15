using Mos3ef.BLL.Dtos.Review;
using Mos3ef.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mos3ef.BLL.Manager.ReviewManager
{
    public interface IReviewManger
    {
        Task<IEnumerable<ReviewReadDto>> GetReviewsByServiceIdAsync(int serviceId);
        Task<bool> AddReviewAsync(ReviewAddDto review);
        Task<bool> UpdateReviewAsync(ReviewUpdateDto review);
        Task<bool> DeleteReviewAsync(int Id);
    }
}
