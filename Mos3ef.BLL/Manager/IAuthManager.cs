using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mos3ef.BLL.Dtos.Auth;

namespace Mos3ef.BLL.Manager
{
    public interface IAuthManager
    {
        Task<AuthResponseDto> RegisterPatientAsync(PatientRegisterDto dto);
        Task<AuthResponseDto> RegisterHospitalAsync(HospitalRegisterDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
    }
}
