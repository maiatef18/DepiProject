using Mos3ef.BLL.Dtos.Compare;
using Mos3ef.BLL.Dtos.Common;
using Mos3ef.BLL.Dtos.Hospital;
using Mos3ef.BLL.Dtos.Review;
using Mos3ef.BLL.Dtos.Services;
using Mos3ef.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mos3ef.BLL.Manager.ServiceManager
{
    public interface IServiceManager
    {
        Task<IEnumerable<ServiceReadDto>> GetServices();
        Task<ServiceReadDto> GetService(int id);
        Task<IEnumerable<ReviewReadDto>> GetServiceReviews(int id);
        Task<HospitalReadDto> GetServiceHospital(int id);
        Task<IEnumerable<ServiceReadDto>> SearchServicesAsync(string keyword);
        Task<PagedResult<ServiceReadDto>> FilterServicesAsync(ServiceFilterDto filterDto);

        Task<ServiceReadDto?> GetByIdAsync(int serviceId);

        Task<CompareResponseDto> CompareServicesAsync(CompareRequestDto dto);
    }
}
