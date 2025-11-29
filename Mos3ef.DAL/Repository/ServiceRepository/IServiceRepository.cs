using Mos3ef.DAL.Enum;
using Mos3ef.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mos3ef.DAL.Repository.ServiceRepository
{
    public interface IServiceRepository
    {
        Task<IEnumerable<Service>> GetServices();
        Task<Service> GetService(int id);
        Task<IEnumerable<Review>> GetServiceReviews(int id);
        Task<Hospital?> GetServiceHospital(int id);
        Task<IEnumerable<Service>> SearchByKeywordAsync(string keyword);
        Task<IEnumerable<Service>> SearchByCategoryAsync(CategoryType category);

        Task<Service?> GetByIdAsync(int serviceId);
    }
}
