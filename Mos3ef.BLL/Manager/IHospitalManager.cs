using Mos3ef.BLL.Dtos.Hospital;
using Mos3ef.BLL.Dtos.Review;
using Mos3ef.BLL.Dtos.Services;
using Mos3ef.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mos3ef.BLL.Manager
{
    public  interface IHospitalManager
    {
        IEnumerable<HospitalReadDto> GetAll();
        HospitalReadDto? Get(int id);
        void Add(HospitalAddDto hospital);
        void Update(HospitalUpdateDto hospital);
        void Delete( int Id);

        void AddService(ServicesAddDto service);
        void UpdateService(ServicesUpdateDto service);
        void DeleteService( int id );

        IQueryable<ReviewReadDto> GetServicesReviews(int hospitalId);

        (int servicesCount, int reviewsCount, double avgRating) GetDashboardStats(int hospitalId);

    }
}
