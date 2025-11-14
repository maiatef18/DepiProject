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
        Task<int> AddReviewAsync(ReviewAddDto review);
        Task<int> UpdateReviewAsync(ReviewUpdateDto review);
        Task DeleteReviewAsync(int Id);
    }
}
