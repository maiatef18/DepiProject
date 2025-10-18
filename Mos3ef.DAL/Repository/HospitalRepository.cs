using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Mos3ef.DAL.Database;
using Mos3ef.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mos3ef.DAL.Repository
{
    public class HospitalRepository : IHospitalRepository
    {
        private readonly ApplicationDbContext _Context;
        public HospitalRepository(ApplicationDbContext applicationDbContext)
        {
            _Context = applicationDbContext;
        }

        public void Add(Hospital hospital)
        {
            _Context.Add(hospital);
            _Context.SaveChanges();
        }
        public void Update(Hospital hospital)
        {
            _Context.SaveChanges();
        }
        public void Delete(Hospital hospital)
        {
            _Context.Remove(hospital);
            _Context.SaveChanges();
        }

        public Hospital? Get(int id)
        {
            return (_Context.Hospitals.Find(id));
        }

        public IQueryable<Hospital> GetAll()
        {
            return (_Context.Hospitals);
        }

        public void AddService(Service service)
        {
            _Context.Services.Add(service);
            _Context.SaveChanges();
        }
        public void UpdateService(Service service)
        {
            _Context.SaveChanges();
        }
        public Service? GetService(int id)
        {
            return (_Context.Services.Find(id));
        }
        public void DeleteService(Service service)
        {
            _Context.Services.Remove(service);
            _Context.SaveChanges();
        }

        public (int servicesCount, int reviewsCount, double avgRating) GetDashboardStats(int hospitalId)
        {
            int servicesCount = _Context.Services.Count(s => s.HospitalId == hospitalId);

            int reviewsCount = _Context.Reviews
                .Count(r => r.Service.HospitalId == hospitalId);

            double avgRating = _Context.Reviews
        .Where(r => r.Service.HospitalId == hospitalId)
        .Select(r => (double?)r.Rating)
        .Average() ?? 0;

            return (servicesCount, reviewsCount, avgRating);
        }

        public IQueryable<Review> GetServicesReviews(int hospitalId)
        {
            return _Context.Services
             .Where(s => s.HospitalId == hospitalId)
             .SelectMany(s => s.Reviews);
        }

    }
}
