using Mos3ef.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mos3ef.DAL.Repository
{
    public interface IHospitalRepository
    {
        IQueryable<Hospital> GetAll();
        Hospital? Get(int id);
        void Add(Hospital hospital);
        void Update(Hospital hospital);

        Service GetService(int id);
        void Delete(Hospital hospital);

        void AddService(Service service);
        void UpdateService(Service service);
        void DeleteService(Service service );

        IQueryable<Review> GetServicesReviews(int hospitalId);

        (int servicesCount, int reviewsCount, double avgRating) GetDashboardStats(int hospitalId);

    }
}
