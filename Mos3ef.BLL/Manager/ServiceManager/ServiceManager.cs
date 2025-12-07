using Mos3ef.DAL.Models;
using Mos3ef.DAL.Repository.ServiceRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mos3ef.BLL.Dtos.Services;
using Mos3ef.BLL.Dtos.Compare;
using Mos3ef.BLL.Dtos.Common;
using AutoMapper;
using Mos3ef.BLL.Dtos.Review;
using Mos3ef.BLL.Dtos.Hospital;
using ComparisonMetrics = Mos3ef.BLL.Dtos.Compare.ComparisonMetrics;
using Mos3ef.DAL.Enum;
using Microsoft.Extensions.Caching.Memory;
using Mos3ef.DAL;
using Mos3ef.Api.Exceptions;

namespace Mos3ef.BLL.Manager.ServiceManager
{
    /// <summary>
    /// Manager for service-related business logic.
    /// 
    /// Following Clean Architecture:
    /// - Throws business exceptions (NotFoundException) when entities are not found
    /// - Controller/Middleware translates these to HTTP responses
    /// - Contains caching logic for performance
    /// </summary>
    public class ServiceManager : IServiceManager
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        public ServiceManager(IServiceRepository serviceRepository, IMapper mapper, IMemoryCache cache)
        {
            _serviceRepository = serviceRepository;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<IEnumerable<ServiceReadDto>> GetServices()
        {
            var services = await _serviceRepository.GetServices();
            return _mapper.Map<IEnumerable<ServiceReadDto>>(services);
        }

        public async Task<ServiceReadDto> GetService(int id)
        {
            var service = await _serviceRepository.GetService(id);
            return _mapper.Map<ServiceReadDto>(service);
        }

        /// <summary>
        /// Get reviews for a service with caching.
        /// </summary>
        public async Task<IEnumerable<ReviewReadDto>> GetServiceReviews(int id)
        {
            var cacheKey = $"{CacheConstant.ServiceReviewsPrefix}{id}";
            
            if (_cache.TryGetValue(cacheKey, out IEnumerable<ReviewReadDto> cachedReviews))
            {
                return cachedReviews;
            }
            
            var reviews = await _serviceRepository.GetServiceReviews(id);
            var reviewDtos = _mapper.Map<IEnumerable<ReviewReadDto>>(reviews);
            
            _cache.Set(cacheKey, reviewDtos, TimeSpan.FromMinutes(5));
            
            return reviewDtos;
        }

        /// <summary>
        /// Get hospital for a service with caching.
        /// </summary>
        /// <exception cref="NotFoundException">Thrown when hospital not found</exception>
        public async Task<HospitalReadDto> GetServiceHospital(int id)
        {
            var cacheKey = $"{CacheConstant.ServiceHospitalPrefix}{id}";
            
            if (_cache.TryGetValue(cacheKey, out HospitalReadDto cachedHospital))
            {
                return cachedHospital;
            }
            
            var hospital = await _serviceRepository.GetServiceHospital(id);
            if (hospital == null)
            {
                throw new NotFoundException($"Hospital not found for service with ID {id}.");
            }
            
            var hospitalDto = _mapper.Map<HospitalReadDto>(hospital);
            _cache.Set(cacheKey, hospitalDto, TimeSpan.FromMinutes(20));
            
            return hospitalDto;
        }

        /// <summary>
        /// Search services by keyword, category, and location with caching.
        /// </summary>
        public async Task<IEnumerable<ServiceReadDto>> SearchServicesAsync(
            string? keyword,
            CategoryType? category,
            double? userLat,
            double? userLon)
        {
            // Create cache key from parameters
            var cacheKey = $"{CacheConstant.ServiceSearchPrefix}" +
                           $"{keyword ?? "all"}_{category?.ToString() ?? "all"}_" +
                           $"{userLat?.ToString() ?? "0"}_{userLon?.ToString() ?? "0"}";
            
            if (_cache.TryGetValue(cacheKey, out IEnumerable<ServiceReadDto> cachedResults))
            {
                return cachedResults;
            }
            
            IEnumerable<Service> services;

            
            if (category.HasValue)
            {
                services = await _serviceRepository.SearchByCategoryAsync(category.Value);
            }
            else if (!string.IsNullOrWhiteSpace(keyword))
            {
                services = await _serviceRepository.SearchByKeywordAsync(keyword);
            }
            else
            {
                return Enumerable.Empty<ServiceReadDto>();
            }

            
            var dtos = _mapper.Map<List<ServiceReadDto>>(services);

            
            for (int i = 0; i < dtos.Count; i++)
            {
                var s = services.ElementAt(i);

                
                if (s.Reviews?.Any() == true)
                    dtos[i].AverageRating = s.Reviews.Average(r => r.Rating);

                
                if (userLat.HasValue && userLon.HasValue &&
                    s.Hospital?.Latitude != null &&
                    s.Hospital?.Longitude != null)
                {
                    dtos[i].DistanceKm = CalculateDistance(
                        userLat.Value, userLon.Value,
                        s.Hospital.Latitude.Value,
                        s.Hospital.Longitude.Value
                    );
                }
            }

            var results = dtos
                .OrderByDescending(d => d.Availability?.ToLower() == "available") 
                .ThenBy(d => d.DistanceKm)                                        
                .ToList();
            
            // Cache for 10 minutes
            _cache.Set(cacheKey, results, TimeSpan.FromMinutes(10));

            return results;
        }

        /// <summary>
        /// Get service by ID with caching.
        /// </summary>
        /// <exception cref="NotFoundException">Thrown when service not found</exception>
        public async Task<ServiceReadDto> GetByIdAsync(int serviceId)
        {
            var cacheKey = $"{CacheConstant.ServicePrefix}{serviceId}";
            
            if (_cache.TryGetValue(cacheKey, out ServiceReadDto cachedService))
            {
                return cachedService;
            }
            
            var service = await _serviceRepository.GetByIdAsync(serviceId);
            if (service == null)
            {
                throw new NotFoundException($"Service with ID {serviceId} not found.");
            }
            
            var serviceDto = _mapper.Map<ServiceReadDto>(service);
            _cache.Set(cacheKey, serviceDto, TimeSpan.FromMinutes(15));
            
            return serviceDto;
        }

        /// <summary>
        /// Compare two services side-by-side.
        /// </summary>
        /// <exception cref="NotFoundException">Thrown when one or both services not found</exception>
        public async Task<CompareResponseDto> CompareServicesAsync(CompareRequestDto dto)
        {
            ServiceReadDto service1;
            ServiceReadDto service2;
            
            try
            {
                service1 = await GetByIdAsync(dto.Service1Id);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException($"Service with ID {dto.Service1Id} not found.");
            }
            
            try
            {
                service2 = await GetByIdAsync(dto.Service2Id);
            }
            catch (NotFoundException)
            {
                throw new NotFoundException($"Service with ID {dto.Service2Id} not found.");
            }

            var metrics = new ComparisonMetrics();

            // Price Comparison
            if (service1.Price != service2.Price)
            {
                metrics.PriceDifference = Math.Abs(service1.Price - service2.Price);
                if (service1.Price < service2.Price)
                {
                    metrics.PriceComparison = $"Service 1 is {metrics.PriceDifference:C} cheaper";
                }
                else
                {
                    metrics.PriceComparison = $"Service 2 is {metrics.PriceDifference:C} cheaper";
                }
            }
            else
            {
                metrics.PriceComparison = "Both services have the same price";
            }

            // Rating Comparison
            if (service1.AverageRating.HasValue && service2.AverageRating.HasValue)
            {
                metrics.RatingDifference = Math.Abs(service1.AverageRating.Value - service2.AverageRating.Value);
                if (service1.AverageRating.Value > service2.AverageRating.Value)
                {
                    metrics.RatingComparison = $"Service 1 has {metrics.RatingDifference:F1} points higher rating";
                }
                else if (service2.AverageRating.Value > service1.AverageRating.Value)
                {
                    metrics.RatingComparison = $"Service 2 has {metrics.RatingDifference:F1} points higher rating";
                }
                else
                {
                    metrics.RatingComparison = "Both services have the same rating";
                }
            }
            else if (service1.AverageRating.HasValue)
            {
                metrics.RatingComparison = "Service 1 has ratings, Service 2 has no ratings yet";
            }
            else if (service2.AverageRating.HasValue)
            {
                metrics.RatingComparison = "Service 2 has ratings, Service 1 has no ratings yet";
            }
            else
            {
                metrics.RatingComparison = "Neither service has ratings yet";
            }

            // Distance Comparison
            if (service1.DistanceKm.HasValue && service2.DistanceKm.HasValue)
            {
                metrics.DistanceDifference = Math.Abs(service1.DistanceKm.Value - service2.DistanceKm.Value);
                if (service1.DistanceKm.Value < service2.DistanceKm.Value)
                {
                    metrics.DistanceComparison = $"Service 1 is {metrics.DistanceDifference:F2} km closer";
                }
                else if (service2.DistanceKm.Value < service1.DistanceKm.Value)
                {
                    metrics.DistanceComparison = $"Service 2 is {metrics.DistanceDifference:F2} km closer";
                }
                else
                {
                    metrics.DistanceComparison = "Both services are at the same distance";
                }
            }

            // Availability Comparison
            var service1Available = service1.Availability?.ToLower() == "available";
            var service2Available = service2.Availability?.ToLower() == "available";
            
            if (service1Available && service2Available)
            {
                metrics.AvailabilityComparison = "Both services are available";
            }
            else if (service1Available)
            {
                metrics.AvailabilityComparison = "Service 1 is available, Service 2 is not";
            }
            else if (service2Available)
            {
                metrics.AvailabilityComparison = "Service 2 is available, Service 1 is not";
            }
            else
            {
                metrics.AvailabilityComparison = "Neither service is currently available";
            }

            // Generate Recommendation
            var recommendationFactors = new List<string>();
            
            if (service1Available && !service2Available)
                recommendationFactors.Add("Service 1 is available");
            else if (service2Available && !service1Available)
                recommendationFactors.Add("Service 2 is available");
            
            if (service1.Price < service2.Price)
                recommendationFactors.Add("Service 1 is more affordable");
            else if (service2.Price < service1.Price)
                recommendationFactors.Add("Service 2 is more affordable");
            
            if (service1.AverageRating.HasValue && service2.AverageRating.HasValue)
            {
                if (service1.AverageRating.Value > service2.AverageRating.Value)
                    recommendationFactors.Add("Service 1 has better ratings");
                else if (service2.AverageRating.Value > service1.AverageRating.Value)
                    recommendationFactors.Add("Service 2 has better ratings");
            }
            
            if (service1.DistanceKm.HasValue && service2.DistanceKm.HasValue)
            {
                if (service1.DistanceKm.Value < service2.DistanceKm.Value)
                    recommendationFactors.Add("Service 1 is closer");
                else if (service2.DistanceKm.Value < service1.DistanceKm.Value)
                    recommendationFactors.Add("Service 2 is closer");
            }

            if (recommendationFactors.Count > 0)
            {
                metrics.Recommendation = string.Join(". ", recommendationFactors) + ".";
            }
            else
            {
                metrics.Recommendation = "Both services are similar. Consider other factors like hospital reputation or specific needs.";
            }

            return new CompareResponseDto
            {
                Service1 = service1,
                Service2 = service2,
                Metrics = metrics
            };
        }

        /// <summary>
        /// Calculate distance between two coordinates using Haversine formula.
        /// </summary>
        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var dLat = (lat2 - lat1) * Math.PI / 180;
            var dLon = (lon2 - lon1) * Math.PI / 180;

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(lat1 * Math.PI / 180) *
                    Math.Cos(lat2 * Math.PI / 180) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return 6371 * c;
        }
    }
}

