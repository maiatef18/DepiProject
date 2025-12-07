using Mos3ef.BLL.Dtos.Patient;
using Mos3ef.BLL.Dtos.Common;
using Mos3ef.BLL.Manager.ServiceManager;
using Mos3ef.DAL.Models;
using Mos3ef.DAL.Repository;
using AutoMapper;
using Mos3ef.BLL.Dtos.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Mos3ef.DAL;
using Mos3ef.Api.Exceptions;

namespace Mos3ef.BLL.Manager.PatientManager
{
    /// <summary>
    /// Manager for patient-related business logic.
    /// 
    /// Following Clean Architecture:
    /// - Throws business exceptions (NotFoundException, BadRequestException) when rules are violated
    /// - Controller/Middleware translates these to HTTP responses
    /// - Contains caching logic for performance
    /// </summary>
    public class PatientManager : IPatientManager
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IServiceManager _serviceManager;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMemoryCache _cache;

        public PatientManager(IPatientRepository patientRepository, IServiceManager serviceManager, IMapper mapper, UserManager<ApplicationUser> userManager, IMemoryCache cache)
        {
            _patientRepository = patientRepository;
            _serviceManager = serviceManager;
            _mapper = mapper;
            _userManager = userManager;
            _cache = cache;
        }

        /// <summary>
        /// Get patient by ID with caching.
        /// </summary>
        /// <exception cref="NotFoundException">Thrown when patient is not found</exception>
        public async Task<PatientReadDto> GetPatientByIdAsync(int id)
        {
            var cacheKey = $"{CacheConstant.PatientProfilePrefix}{id}";
            
            if (_cache.TryGetValue(cacheKey, out PatientReadDto cachedPatient))
            {
                return cachedPatient;
            }
            
            var patient = await _patientRepository.GetPatientByIdAsync(id);
            if (patient == null)
            {
                throw new NotFoundException($"Patient with ID {id} not found.");
            }
            
            var patientDto = _mapper.Map<PatientReadDto>(patient);
            _cache.Set(cacheKey, patientDto, TimeSpan.FromMinutes(5));
            
            return patientDto;
        }

        /// <summary>
        /// Get patient by ApplicationUser ID.
        /// </summary>
        /// <exception cref="NotFoundException">Thrown when patient is not found</exception>
        public async Task<PatientReadDto> GetPatientByUserIdAsync(string userId)
        {
            var patient = await _patientRepository.GetPatientByUserIdAsync(userId);
            if (patient == null)
            {
                throw new NotFoundException("Patient profile not found for this user.");
            }

            return _mapper.Map<PatientReadDto>(patient);
        }

        public async Task<IEnumerable<ServiceReadDto>> GetSavedServicesAsync(int patientId)
        {
            // Verify patient exists
            await GetPatientByIdAsync(patientId);

            var savedServices = await _patientRepository.GetSavedServicesAsync(patientId);
            return _mapper.Map<IEnumerable<ServiceReadDto>>(savedServices.Select(ss => ss.Service));
        }

        /// <summary>
        /// Get saved services with pagination and caching.
        /// </summary>
        public async Task<PagedResult<ServiceReadDto>> GetSavedServicesPagedAsync(int patientId, int pageNumber, int pageSize)
        {
            var cacheKey = $"{CacheConstant.PatientSavedServicesPrefix}{patientId}_p{pageNumber}_s{pageSize}";
            
            if (_cache.TryGetValue(cacheKey, out PagedResult<ServiceReadDto> cachedResult))
            {
                return cachedResult;
            }
            
            var (savedServices, totalCount) = await _patientRepository.GetSavedServicesPagedAsync(patientId, pageNumber, pageSize);
            var services = _mapper.Map<IEnumerable<ServiceReadDto>>(savedServices.Select(ss => ss.Service));

            var result = new PagedResult<ServiceReadDto>
            {
                Items = services,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            
            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(3));
            
            return result;
        }

        /// <summary>
        /// Remove a saved service from patient's list.
        /// </summary>
        /// <exception cref="NotFoundException">Thrown when saved service link not found</exception>
        public async Task RemoveSavedServiceAsync(int patientId, int serviceId)
        {
            var savedService = await _patientRepository.FindSavedServiceAsync(patientId, serviceId);

            if (savedService == null)
            {
                throw new NotFoundException("Saved service link not found.");
            }

            _patientRepository.RemoveSavedService(savedService);
            await _patientRepository.SaveChangesAsync();
            
            // Invalidate saved services cache
            InvalidateSavedServicesCache(patientId);
        }

        /// <summary>
        /// Save a service to patient's favorites list.
        /// </summary>
        /// <exception cref="NotFoundException">Thrown when service not found</exception>
        public async Task SaveServiceAsync(int patientId, int serviceId)
        {
            // Verify patient exists (will throw if not found)
            await GetPatientByIdAsync(patientId);

            // Verify service exists (ServiceManager will throw if not found)
            var service = await _serviceManager.GetByIdAsync(serviceId);
            if (service == null)
            {
                throw new NotFoundException($"Service with ID {serviceId} not found.");
            }

            // Check if already saved (idempotent operation)
            var alreadyExists = await _patientRepository.FindSavedServiceAsync(patientId, serviceId);
            if (alreadyExists != null)
            {
                return; // Already saved, nothing to do
            }

            var savedService = new SavedService
            {
                PatientId = patientId,
                ServiceId = serviceId
            };

            await _patientRepository.AddSavedServiceAsync(savedService);
            await _patientRepository.SaveChangesAsync();
            
            // Invalidate saved services cache
            InvalidateSavedServicesCache(patientId);
        }

        /// <summary>
        /// Update patient profile.
        /// </summary>
        /// <exception cref="NotFoundException">Thrown when patient not found</exception>
        /// <exception cref="InvalidOperationException">Thrown when email already in use</exception>
        public async Task UpdatePatientAsync(int id, PatientUpdateDto patientUpdateDto, string? imagePath = null)
        {
            // Use ForUpdate method to get tracked entity
            var patient = await _patientRepository.GetPatientByIdForUpdateAsync(id);

            if (patient == null)
            {
                throw new NotFoundException($"Patient with ID {id} not found.");
            }

            // Update Patient fields (Name, Address)
            _mapper.Map(patientUpdateDto, patient);

            // Update profile picture if provided
            if (!string.IsNullOrEmpty(imagePath))
            {
                patient.ImageUrl = imagePath;
            }

            // Update ApplicationUser fields (Email, PhoneNumber)
            var user = await _userManager.FindByIdAsync(patient.UserId);
            if (user != null)
            {
                bool userUpdated = false;

                if (!string.IsNullOrEmpty(patientUpdateDto.Email) && user.Email != patientUpdateDto.Email)
                {
                    // Check if email is already taken by another user
                    var existingUser = await _userManager.FindByEmailAsync(patientUpdateDto.Email);
                    if (existingUser != null && existingUser.Id != user.Id)
                    {
                        throw new InvalidOperationException("Email is already in use by another account.");
                    }

                    user.Email = patientUpdateDto.Email;
                    user.UserName = patientUpdateDto.Email; // Keep username in sync with email
                    userUpdated = true;
                }

                if (!string.IsNullOrEmpty(patientUpdateDto.PhoneNumber) && user.PhoneNumber != patientUpdateDto.PhoneNumber)
                {
                    user.PhoneNumber = patientUpdateDto.PhoneNumber;
                    userUpdated = true;
                }

                if (userUpdated)
                {
                    var result = await _userManager.UpdateAsync(user);
                    if (!result.Succeeded)
                    {
                        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                        throw new InvalidOperationException($"Failed to update user information: {errors}");
                    }
                }
            }

            await _patientRepository.SaveChangesAsync();
            
            // Invalidate patient profile cache
            var cacheKey = $"{CacheConstant.PatientProfilePrefix}{id}";
            _cache.Remove(cacheKey);
        }
        
        private void InvalidateSavedServicesCache(int patientId)
        {
            // Invalidate cache for common page sizes
            for (int page = 1; page <= 10; page++)
            {
                foreach (int size in new[] { 5, 10, 20, 50 })
                {
                    var cacheKey = $"{CacheConstant.PatientSavedServicesPrefix}{patientId}_p{page}_s{size}";
                    _cache.Remove(cacheKey);
                }
            }
        }
    }
}
