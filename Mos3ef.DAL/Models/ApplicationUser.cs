using Microsoft.AspNetCore.Identity;
using Mos3ef.DAL.Enums;

namespace Mos3ef.DAL.Models
{
    public class ApplicationUser : IdentityUser<int>
    {
        public UserType UserType { get; set; }

        public Patient? PatientProfile { get; set; }
        public Hospital? HospitalProfile { get; set; }
    }
}
