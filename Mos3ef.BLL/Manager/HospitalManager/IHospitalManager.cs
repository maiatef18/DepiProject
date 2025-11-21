using Mos3ef.BLL.Dtos.Hospital;
using Mos3ef.BLL.Dtos.Review;
using Mos3ef.BLL.Dtos.Services;
using Mos3ef.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mos3ef.BLL.Manager.HospitalManager
{
    public  interface IHospitalManager
    {
        Task<IEnumerable<HospitalReadDto>> GetAllAsync();
        Task<HospitalReadDto?> GetAsync(int id);
        Task<int> AddAsync(HospitalAddDto hospital);
        Task UpdateAsync(HospitalUpdateDto hospital);
        Task DeleteAsync(int id);

 
        Task<int> AddServiceAsync( string userID , ServicesAddDto service);
        Task UpdateServiceAsync(ServicesUpdateDto service);
        Task DeleteServiceAsync(int id);

 
        Task<IEnumerable<ReviewReadDto>> GetServicesReviewsAsync(int hospitalId);


        Task<(int servicesCount, int reviewsCount, double avgRating)> GetDashboardStatsAsync(int hospitalId);
    }
}

