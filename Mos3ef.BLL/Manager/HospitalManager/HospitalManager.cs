using AutoMapper;
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

        public HospitalManager(IHospitalRepository hospitalRepository, IMapper mapper)
        {
            _hospitalRepository = hospitalRepository;
            _mapper = mapper;
        }

        public async Task<int> AddAsync(HospitalAddDto hospital)
        {
            var entity = _mapper.Map<Hospital>(hospital);
            var id = await _hospitalRepository.AddAsync(entity);
            return id;
        }

        public async Task<int> AddServiceAsync(ServicesAddDto service)
        {
            var entity = _mapper.Map<Service>(service);
            var id = await _hospitalRepository.AddServiceAsync(entity);
            return id;
        }

        public async Task DeleteAsync(int id)
        {
            var hospital = await _hospitalRepository.GetAsync(id);
            if (hospital == null)
                throw new Exception("Hospital not found");

            await _hospitalRepository.DeleteAsync(hospital);
        }

        public async Task DeleteServiceAsync(int id)
        {
            var service = await _hospitalRepository.GetServiceAsync(id);
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

        public async Task UpdateAsync(HospitalUpdateDto hospital)
        {
            var entity = await _hospitalRepository.GetAsync(hospital.Id);
            if (entity == null)
                throw new Exception("Hospital not found");

            _mapper.Map(hospital, entity);
            await _hospitalRepository.UpdateAsync(entity);
        }

        public async Task UpdateServiceAsync(ServicesUpdateDto service)
        {
            var entity = await _hospitalRepository.GetServiceAsync(service.ServiceId);
            if (entity == null)
                throw new Exception("Service not found");

            _mapper.Map(service, entity);
            await _hospitalRepository.UpdateServiceAsync(entity);
        }
    }
}
