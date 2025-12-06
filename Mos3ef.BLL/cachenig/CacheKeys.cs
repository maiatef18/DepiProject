using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mos3ef.BLL.cachenig
{
    public static class CacheKeys
    {

        public static string Hospital(string id) => $"hospital:{id}";
        public static string AllHospitals => $"hospitals:all";

        public static string HospitalServices(string hospitalId) => $"hospital:{hospitalId}:services";
        public static string HospitalReviews(int hospitalId) => $"hospital:{hospitalId}:reviews";
        public static string Dashboard(int hospitalId) => $"hospital:{hospitalId}:dashboard";
    }
}
