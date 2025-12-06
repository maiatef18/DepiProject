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
        Task<HospitalReadDto?> GetAsync(String id);
        Task<Hospital> AddAsync(HospitalAddDto hospital);
        Task<HospitalReadDto?> UpdateAsync(string  Hospital_Id, HospitalUpdateDto hospital);
        Task DeleteAsync(string id);

 
        Task<ServiceShowDto> AddServiceAsync( string userID , ServicesAddDto service);
        Task<ServiceShowDto> UpdateServiceAsync( string Hospital_id , int id , ServicesUpdateDto service);
        Task DeleteServiceAsync(string Hospital_ID , int id);
        Task<IEnumerable<ServiceHospitalDto>> GetAllServicesAsync(string Hospital_ID);

        Task<IEnumerable<ReviewReadDto>> GetServicesReviewsAsync(int hospitalId);


        Task<(int servicesCount, int reviewsCount, double avgRating)> GetDashboardStatsAsync(int hospitalId);
    }
}

