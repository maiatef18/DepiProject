using Mos3ef.BLL.Dtos.Hospital;
using Mos3ef.BLL.Dtos.Review;
using Mos3ef.BLL.Dtos.Services;
using Mos3ef.DAL.Models;
using Mos3ef.DAL.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mos3ef.BLL.Manager
{
    public class HospitalManager : IHospitalManager
    {
        private readonly IHospitalRepository _hospitalRepository;

        public HospitalManager(IHospitalRepository hospitalRepository) 
        {
            _hospitalRepository = hospitalRepository;
        }
        public void Add(HospitalAddDto hospital)
        {
            var HospitalAdd = new Hospital
            {
                Name = hospital.Name,
                Description = hospital.Description,
                Opening_Hours = hospital.Opening_Hours,
                Phone_Number = hospital.Phone_Number,
                Address = hospital.Address,
                Location = hospital.Location,
                Website = hospital.Website,
            };
            _hospitalRepository.Add(HospitalAdd);
        }

        public void AddService(ServicesAddDto service)
        {
            var ServiceAdd = new Service
            {
                Name = service.Name,
                Description = service.Description,
                Price = service.Price,
                Availability = service.Availability,
                Working_Hours = service.Working_Hours,
                Category = service.Category
            };
            _hospitalRepository.AddService(ServiceAdd);
        }

        public void Delete(int Id)
        {
            var Hospital = _hospitalRepository.Get(Id);
            _hospitalRepository.Delete(Hospital);
        }

        public void DeleteService(int id)
        {
            var service = _hospitalRepository.GetService(id);
            _hospitalRepository.DeleteService(service);
        }

        public HospitalReadDto? Get(int id)
        {
            var Hospital = _hospitalRepository.Get(id);
            var HospitalRead = new HospitalReadDto
            {
                Name = Hospital.Name,
                Description = Hospital.Description,
                Opening_Hours = Hospital.Opening_Hours,
                Phone_Number = Hospital.Phone_Number,
                Address = Hospital.Address,
                Location = Hospital.Location,
                Website = Hospital.Website,
                Services = Hospital.Services
                    .Where(s => s.HospitalId == Hospital.HospitalId)
                    .Select(s => new ServiceReadDto
                    {
                        Name = s.Name,
                        Price = s.Price,
                        Description = s.Description,
                        Availability = s.Availability,
                        Working_Hours = s.Working_Hours,
                        Category = s.Category
                    }).ToList()
            };
            return HospitalRead;
        }

        public IEnumerable<HospitalReadDto> GetAll()
        {
            var Hospitals = _hospitalRepository.GetAll();

            var HospitalReads = Hospitals.Select(x => new HospitalReadDto
            {
                Name = x.Name,
                Description = x.Description,
                Location = x.Location,
                Address = x.Address,
                Opening_Hours = x.Opening_Hours,
                Phone_Number = x.Phone_Number,
                Website = x.Website,
                Services = x.Services
                    .Where(s => s.HospitalId == x.HospitalId)
                    .Select(s => new ServiceReadDto
                    {
                        Name = s.Name,
                        Price = s.Price,
                        Description = s.Description,
                        Availability = s.Availability,
                        Working_Hours = s.Working_Hours,
                        Category = s.Category
                    }).ToList()
            });
            return HospitalReads;
        }

        public (int servicesCount, int reviewsCount, double avgRating) GetDashboardStats(int hospitalId)
        {
            return _hospitalRepository.GetDashboardStats(hospitalId);

        }

        public IQueryable<ReviewReadDto> GetServicesReviews(int hospitalId)
        {
            return (IQueryable<ReviewReadDto>)_hospitalRepository.GetServicesReviews(hospitalId).ToList();
        }

        public void Update(HospitalUpdateDto hospital)
        {
            var HospitalUpdate = _hospitalRepository.Get(hospital.Id);

            HospitalUpdate = new Hospital
            {
                Name = hospital.Name,
                Description = hospital.Description,
                Opening_Hours = hospital.Opening_Hours,
                Website = hospital.Website,
                Address = hospital.Address,
                Location = hospital.Location,
                Phone_Number = hospital.Phone_Number,

            };
            _hospitalRepository.Update(HospitalUpdate);
        }

        public void UpdateService(ServicesUpdateDto service)
        {
            var ServiceUpdate = _hospitalRepository.GetService(service.ServiceId);

            ServiceUpdate = new Service
            {
                Name = service.Name,
                Description = service.Description,
                Price = service.Price,
                Availability = service.Availability,
                Working_Hours = service.Working_Hours,
                Category = service.Category

            };
            _hospitalRepository.UpdateService(ServiceUpdate);
        }

        IEnumerable<HospitalReadDto> IHospitalManager.GetAll()
        {
            return GetAll();
        }

        IQueryable<ReviewReadDto> IHospitalManager.GetServicesReviews(int hospitalId)
        {
            return GetServicesReviews(hospitalId);
        }
    }
}
