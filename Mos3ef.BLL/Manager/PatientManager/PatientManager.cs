using Mos3ef.BLL.Dtos.Patient;
using Mos3ef.BLL.Dtos.Common;
using Mos3ef.BLL.Manager.ServiceManager;
using Mos3ef.DAL.Models;
using Mos3ef.DAL.Repository;
using AutoMapper;
using Mos3ef.BLL.Dtos.Services;

namespace Mos3ef.BLL.Manager.PatientManager
{
    public class PatientManager : IPatientManager
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IServiceManager _serviceManager;
        private readonly IMapper _mapper;

        public PatientManager(IPatientRepository patientRepository, IServiceManager serviceManager, IMapper mapper)
        {
            _patientRepository = patientRepository;
            _serviceManager = serviceManager;
            _mapper = mapper;
        }

        public async Task<PatientReadDto?> GetPatientByIdAsync(int id)
        {
            var patient = await _patientRepository.GetPatientByIdAsync(id);
            return _mapper.Map<PatientReadDto>(patient);
        }

        public async Task<PatientReadDto?> GetPatientByUserIdAsync(string userId)
        {
            var patient = await _patientRepository.GetPatientByUserIdAsync(userId);
            return _mapper.Map<PatientReadDto>(patient);
        }

        public async Task<IEnumerable<ServiceReadDto>> GetSavedServicesAsync(int patientId)
        {
            var patient = await _patientRepository.GetPatientByIdAsync(patientId);
            if (patient == null)
            {
                return Enumerable.Empty<ServiceReadDto>();
            }

            var savedServices = await _patientRepository.GetSavedServicesAsync(patientId);
            return _mapper.Map<IEnumerable<ServiceReadDto>>(savedServices.Select(ss => ss.Service));
        }

        public async Task<PagedResult<ServiceReadDto>> GetSavedServicesPagedAsync(int patientId, int pageNumber, int pageSize)
        {
            var patient = await _patientRepository.GetPatientByIdAsync(patientId);
            if (patient == null)
            {
                return new PagedResult<ServiceReadDto>
                {
                    Items = Enumerable.Empty<ServiceReadDto>(),
                    TotalCount = 0,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }

            var (savedServices, totalCount) = await _patientRepository.GetSavedServicesPagedAsync(patientId, pageNumber, pageSize);
            var services = _mapper.Map<IEnumerable<ServiceReadDto>>(savedServices.Select(ss => ss.Service));

            return new PagedResult<ServiceReadDto>
            {
                Items = services,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<bool> RemoveSavedServiceAsync(int patientId, int serviceId)
        {
            var savedService = await _patientRepository.FindSavedServiceAsync(patientId, serviceId);

            if (savedService == null)
            {
                return false; 
            }


            _patientRepository.RemoveSavedService(savedService);
            await _patientRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SaveServiceAsync(int patientId, int serviceId)
        {
 
            var patient = await _patientRepository.GetPatientByIdAsync(patientId);
            if (patient == null) return false;

            var service = await _serviceManager.GetByIdAsync(serviceId);
            if (service == null) return false; 


            var alreadyExists = await _patientRepository.FindSavedServiceAsync(patientId, serviceId);
            if (alreadyExists != null)
            {
                return true; 
            }

            var savedService = new SavedService
            {
                PatientId = patientId,
                ServiceId = serviceId
            };

            await _patientRepository.AddSavedServiceAsync(savedService);
            await _patientRepository.SaveChangesAsync(); 
            return true;
        }

        public async Task<bool> UpdatePatientAsync(int id, PatientUpdateDto patientUpdateDto)
        {
            var patient = await _patientRepository.GetPatientByIdAsync(id);

            if (patient == null)
            {
                return false; 
            }


            _mapper.Map(patientUpdateDto, patient);

            await _patientRepository.SaveChangesAsync();
            return true;
        }
    }
}