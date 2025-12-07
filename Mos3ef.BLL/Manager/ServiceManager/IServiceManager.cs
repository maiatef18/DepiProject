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
using Mos3ef.DAL.Enum;

namespace Mos3ef.BLL.Manager.ServiceManager
{
    /// <summary>
    /// Interface for service-related business operations.
    /// Methods throw exceptions for error cases instead of returning null.
    /// </summary>
    public interface IServiceManager
    {
        Task<IEnumerable<ServiceReadDto>> GetServices();
        Task<ServiceReadDto> GetService(int id);
        Task<IEnumerable<ReviewReadDto>> GetServiceReviews(int id);
        
        /// <exception cref="Mos3ef.Api.Exceptions.NotFoundException">Thrown when hospital not found</exception>
        Task<HospitalReadDto> GetServiceHospital(int id);
        
        Task<IEnumerable<ServiceReadDto>> SearchServicesAsync(
            string? keyword,
            CategoryType? category,
            double? userLat,
            double? userLon);
        
        /// <exception cref="Mos3ef.Api.Exceptions.NotFoundException">Thrown when service not found</exception>
        Task<ServiceReadDto> GetByIdAsync(int serviceId);

        /// <exception cref="Mos3ef.Api.Exceptions.NotFoundException">Thrown when one or both services not found</exception>
        Task<CompareResponseDto> CompareServicesAsync(CompareRequestDto dto);
    }
}

