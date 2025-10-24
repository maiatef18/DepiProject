using Mos3ef.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mos3ef.DAL.Repository
{
    public interface IHospitalRepository
    {
    
        Task<IEnumerable<Hospital>> GetAllAsync();
        Task<Hospital?> GetAsync(int id);
        Task<int> AddAsync(Hospital hospital);
        Task UpdateAsync(Hospital hospital);
        Task DeleteAsync(Hospital hospital);
    
        Task<Service?> GetServiceAsync(int id);
        Task<int> AddServiceAsync(Service service);
        Task UpdateServiceAsync(Service service);
        Task DeleteServiceAsync(Service service);
    
        Task<IEnumerable<Review>> GetServicesReviewsAsync(int hospitalId);


        Task<(int ServicesCount, int ReviewsCount, double AvgRating)> GetDashboardStatsAsync(int hospitalId);
    }

}
