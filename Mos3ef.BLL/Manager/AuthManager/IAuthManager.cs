using System.Threading.Tasks;
using Mos3ef.BLL.Dtos.Auth;
using Mos3ef.DAL.Wapper;

namespace Mos3ef.BLL.Manager.AuthManager
{
    public interface IAuthManager
    {
        Task<Response<AuthResponseDto>> RegisterPatientAsync(PatientRegisterDto dto);
        Task<Response<AuthResponseDto>> RegisterHospitalAsync(HospitalRegisterDto dto);
        Task<Response<AuthResponseDto>> LoginAsync(LoginDto dto);
        Task<Response<string>> ChangePasswordAsync(string userId, ChangePasswordDto dto);
        Task<Response<string>> LogoutAsync(string token);
    }
}
