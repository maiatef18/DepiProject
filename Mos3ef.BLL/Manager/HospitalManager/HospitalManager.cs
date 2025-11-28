using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        public HospitalManager(IHospitalRepository hospitalRepository, IMapper mapper , IWebHostEnvironment env)
        {
            _hospitalRepository = hospitalRepository;
            _mapper = mapper;
            _env = env;
        }

        public async Task<int> AddAsync(HospitalAddDto hospital)
        {
            var entity = _mapper.Map<Hospital>(hospital);
            var id = await _hospitalRepository.AddAsync(entity);
            return id;
        }

        public async Task DeleteAsync(string id)
        {
            var hospital = await _hospitalRepository.GetHospitalAsync(id);
            if (hospital == null)
                throw new Exception("Hospital not found");

            await _hospitalRepository.DeleteAsync(hospital);
        }

        public async Task DeleteServiceAsync(string Hospital_ID , int id)
        {
            var service = await _hospitalRepository.GetServiceAsync(Hospital_ID , id);
            if (service == null)
                throw new Exception("Service not found");

            await _hospitalRepository.DeleteServiceAsync(service);
        }

        public async Task<HospitalReadDto?> GetAsync(int id)
        {
            var hospital = await _hospitalRepository.GetAsync(id);
            return hospital == null ? null : _mapper.Map<HospitalReadDto>(hospital);
        }

        public async Task<IEnumerable<HospitalReadDto>> GetAllAsync()
        {
            var hospitals = await  _hospitalRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<HospitalReadDto>>(hospitals);
        }

        public async Task<(int servicesCount, int reviewsCount, double avgRating)> GetDashboardStatsAsync(int hospitalId)
        {
            return await _hospitalRepository.GetDashboardStatsAsync(hospitalId);
        }

        public async Task<IEnumerable<ReviewReadDto>> GetServicesReviewsAsync(int hospitalId)
        {
            var reviews = await _hospitalRepository.GetServicesReviewsAsync(hospitalId);
            return _mapper.Map<IEnumerable<ReviewReadDto>>(reviews);
        }

        public async Task UpdateAsync(string hospitalId, HospitalUpdateDto dto)
        {
            var entity = await _hospitalRepository.GetHospitalAsync(hospitalId);

            if (entity == null)
                throw new Exception("Hospital not found");

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

            await _hospitalRepository.UpdateAsync();
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


        public async Task UpdateServiceAsync(string Hospital_Id , int id , ServicesUpdateDto service)
        {
            var entity = await _hospitalRepository.GetServiceAsync(Hospital_Id, id);
            if (entity == null)
                throw new Exception("Service not found"); 

            _mapper.Map(service, entity);
            await _hospitalRepository.UpdateServiceAsync();
        }

        public async Task<int> AddServiceAsync(string userId , ServicesAddDto service)
        {
            int hospitalId = await _hospitalRepository.GetHospitalIdByUserIdAsync(userId);

            if (hospitalId == 0)
                throw new Exception("Hospital not registered!");

            var entity = _mapper.Map<Service>(service);
            entity.HospitalId = hospitalId;

            var id = await _hospitalRepository.AddServiceAsync(entity);
            return id;
        }

        
    }
}
