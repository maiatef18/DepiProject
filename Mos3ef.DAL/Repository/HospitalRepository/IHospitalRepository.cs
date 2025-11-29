using Mos3ef.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mos3ef.DAL.Repository.HospitalRepository
{
    public interface IHospitalRepository
    {
        Task<int> GetHospitalIdByUserIdAsync(string userId);
        Task<IEnumerable<Hospital>> GetAllAsync();
        Task<Hospital?> GetAsync(String id);
        Task<Hospital?> GetHospitalAsync(string id);
        Task<int> AddAsync(Hospital hospital);
        Task UpdateAsync();
        Task DeleteAsync(Hospital hospital);
    
        Task<Service?> GetServiceAsync(string Hospital_Id , int id);
        Task<int> AddServiceAsync(Service service);
        Task UpdateServiceAsync();
        Task DeleteServiceAsync(Service service);
    
        Task<IEnumerable<Review>> GetServicesReviewsAsync(int hospitalId);


        Task<(int ServicesCount, int ReviewsCount, double AvgRating)> GetDashboardStatsAsync(int hospitalId);
    }

}
