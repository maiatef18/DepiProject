using Mos3ef.BLL.Dtos.ReviewDto;
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
        IEnumerable<ReviewReadDto> GetReviewsByServiceId(int serviceId);
        void AddReview(ReviewAddDto review);
        void UpdateReview(ReviewUpdateDto review);
        void DeleteReview(int Id);
    }
}
