using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mos3ef.DAL
{
    public static class CacheConstant
    {
        // Patient cache keys
        public const string PatientProfilePrefix = "patient_profile_";
        public const string PatientSavedServicesPrefix = "patient_saved_";

        // Service cache keys
        public const string ServicePrefix = "service_";
        public const string ServiceSearchPrefix = "service_search_";
        public const string ServiceReviewsPrefix = "service_reviews_";
        public const string ServiceHospitalPrefix = "service_hospital_";

        // Review cache keys
        public const string reviewCacheKey = "ReviewCacheKey";
    }
}
