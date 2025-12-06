using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Mos3ef.BLL.cachenig;
using Mos3ef.BLL.Dtos.Hospital;
using Mos3ef.BLL.Dtos.Review;
using Mos3ef.BLL.Dtos.Services;
using Mos3ef.DAL.Models;
using Mos3ef.DAL.Repository.HospitalRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mos3ef.BLL.Manager.HospitalManager
{
    public class HospitalManager : IHospitalManager
    {
        private readonly IHospitalRepository _hospitalRepository;
        private readonly ICacheService _CacheService;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;

        public HospitalManager(
            IHospitalRepository hospitalRepository,
            ICacheService cacheService,
            IWebHostEnvironment env,
            IMapper mapper 
            )
        {
            _hospitalRepository = hospitalRepository;
            _CacheService = cacheService;
            _mapper = mapper;
            _env = env;
        }

        public async Task<IEnumerable<HospitalReadDto>> GetAllAsync()
        {
            string key = CacheKeys.AllHospitals;

            var cached = await _CacheService.GetAsync<IEnumerable<HospitalReadDto>>(key);
            if (cached != null)
                return cached;

            var hospitals = await _hospitalRepository.GetAllAsync();
            var mapped = _mapper.Map<IEnumerable<HospitalReadDto>>(hospitals);

            await _CacheService.SetAsync(key, mapped, TimeSpan.FromMinutes(10));

            return mapped;
        }


        public async Task<Hospital> AddAsync(HospitalAddDto hospital)
        {
            var entity = _mapper.Map<Hospital>(hospital);
            var Hospital = await _hospitalRepository.AddAsync(entity);

            // Invalidate cache
            await _CacheService.RemoveAsync(CacheKeys.AllHospitals);

            return Hospital;
        }

        public async Task DeleteAsync(string Hospital_ID)
        {
            var hospital = await _hospitalRepository.GetHospitalAsync(Hospital_ID);
            if (hospital == null)
                throw new Exception("Hospital not found");

            await _hospitalRepository.DeleteAsync(hospital);

            // Invalidate cache
            await _CacheService.RemoveAsync(CacheKeys.Hospital(Hospital_ID));
            await _CacheService.RemoveAsync(CacheKeys.AllHospitals);
        }

        public async Task DeleteServiceAsync(string Hospital_ID , int Service_ID)
        {
            var service = await _hospitalRepository.GetServiceAsync(Hospital_ID, Service_ID);
            if (service == null)
                throw new Exception("Service not found");

            await _hospitalRepository.DeleteServiceAsync(service);

            // Invalidate service list
            await _CacheService.RemoveAsync(CacheKeys.HospitalServices(Hospital_ID));
        }

        public async Task<HospitalReadDto?> GetAsync(string Hospital_ID)
        {
            string key = CacheKeys.Hospital(Hospital_ID);

            var cached = await _CacheService.GetAsync<HospitalReadDto>(key);
            if (cached != null)
                return cached;

            var hospital = await _hospitalRepository.GetAsync(Hospital_ID);
            if (hospital == null)
                return null;

            var Hospital = _mapper.Map<HospitalReadDto>(hospital);

            await _CacheService.SetAsync(key, Hospital, TimeSpan.FromMinutes(10));

            return Hospital;
        }


        public async Task<(int servicesCount, int reviewsCount, double avgRating)> GetDashboardStatsAsync(int hospitalId)
        {
            string key = CacheKeys.Dashboard(hospitalId);

            var cached = await _CacheService.GetAsync<(int, int, double)?>(key);
            if (cached != null)
                return cached.Value;

            var stats = await _hospitalRepository.GetDashboardStatsAsync(hospitalId);

            await _CacheService.SetAsync(key, stats, TimeSpan.FromMinutes(5));

            return stats;
        }

        public async Task<IEnumerable<ReviewReadDto>> GetServicesReviewsAsync(int hospitalId)
        {
            string key = CacheKeys.HospitalReviews(hospitalId);

            var cached = await _CacheService.GetAsync<IEnumerable<ReviewReadDto>>(key);
            if (cached != null)
                return cached;

            var reviews = await _hospitalRepository.GetServicesReviewsAsync(hospitalId);
            var Reviews = _mapper.Map<IEnumerable<ReviewReadDto>>(reviews);

            await _CacheService.SetAsync(key, Reviews, TimeSpan.FromMinutes(10));

            return Reviews;
        }

        public async Task<HospitalReadDto> UpdateAsync(string hospitalId, HospitalUpdateDto dto)
        {
            var entity = await _hospitalRepository.GetHospitalAsync(hospitalId);

            if (entity == null)
                throw new Exception("Hospital not found");

            // Handle image upload
            if (dto.ProfileImage != null)
            {
                if (!string.IsNullOrEmpty(entity.ImageUrl))
                {
                    string oldImagePath = Path.Combine(_env.WebRootPath, entity.ImageUrl.TrimStart('/'));
                    if (File.Exists(oldImagePath))
                        File.Delete(oldImagePath);
                }

                string newImageUrl = await SaveHospitalImage(dto.ProfileImage);
                entity.ImageUrl = newImageUrl;
            }

            _mapper.Map(dto, entity);

            var Result  = await _hospitalRepository.UpdateAsync(entity);

            // Invalidate cache
            await _CacheService.RemoveAsync(CacheKeys.Hospital(hospitalId));
            await _CacheService.RemoveAsync(CacheKeys.AllHospitals);
            var Hospital = _mapper.Map<HospitalReadDto>(Result);

            return Hospital;


        }

        private async Task<string> SaveHospitalImage(IFormFile file)
        {
            var allowedExt = new[] { ".jpg", ".jpeg", ".png" };
            var ext = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExt.Contains(ext))
                throw new Exception("Invalid image type");

            string fileName = Guid.NewGuid().ToString() + ext;
            string folder = Path.Combine(_env.WebRootPath, "images/hospitals");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string filePath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/images/hospitals/{fileName}";
        }


        public async Task<ServiceShowDto> UpdateServiceAsync(string hospitalId, int serviceId, ServicesUpdateDto service)
        {
            var entity = await _hospitalRepository.GetServiceAsync(hospitalId, serviceId);

            if (entity == null)
                throw new Exception("Service not found");

            _mapper.Map(service, entity);

            var Service = await _hospitalRepository.UpdateServiceAsync(entity);

            // Invalidate services cache
            await _CacheService.RemoveAsync(CacheKeys.HospitalServices(hospitalId));

            return _mapper.Map<ServiceShowDto>(Service);
        }


        public async Task<ServiceShowDto> AddServiceAsync(string userId , ServicesAddDto service)
        {
             var hospitalId = await _hospitalRepository.GetHospitalIdByUserIdAsync(userId);

            if (hospitalId == 0)
                throw new Exception("Hospital not registered!");

            var entity = _mapper.Map<Service>(service);
            entity.HospitalId = hospitalId;

            var Service = await _hospitalRepository.AddServiceAsync(entity);

            // Invalidate service list
            await _CacheService.RemoveAsync(CacheKeys.HospitalServices(hospitalId.ToString()));

            var Entity  = _mapper.Map<ServiceShowDto>(Service);
            return Entity;
        }

        public async Task<IEnumerable<ServiceHospitalDto>> GetAllServicesAsync(string hospitalId)
        {
            string key = CacheKeys.HospitalServices(hospitalId);

            var cached = await _CacheService.GetAsync<IEnumerable<ServiceHospitalDto>>(key);
            if (cached != null)
                return cached;

            var services = await _hospitalRepository.GetAllServiceAsync(hospitalId);
            var Services = _mapper.Map<IEnumerable<ServiceHospitalDto>>(services);

            await _CacheService.SetAsync(key, Services, TimeSpan.FromMinutes(10));

            return Services;
        }

    }
}
