using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mos3ef.DAL.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Mos3ef.DAL.Repository
{
    public interface IAuthRepository
    {
        Task<ApplicationUser?> GetUserByEmailAsync(string email);
        Task<(bool IsSuccess, string? Error)> CreateUserAsync(ApplicationUser user, string password);

        Task AddHospitalProfileAsync(Hospital hospital);

        Task AddPatientProfileAsync(Patient patient);
    }
}
