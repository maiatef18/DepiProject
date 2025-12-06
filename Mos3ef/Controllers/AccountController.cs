using Microsoft.AspNetCore.Mvc;
using Mos3ef.BLL.Dtos.Auth;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;
using Mos3ef.BLL.Manager.AuthManager;
using Mos3ef.DAL.Wapper;

namespace Mos3ef.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthManager _authManager;

        public AccountController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        #region RegisterPatient

        [HttpPost("register/patient")]
        public async Task<IActionResult> RegisterPatient([FromBody] PatientRegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(Response<string>.Fail("Invalid model state."));

            var result = await _authManager.RegisterPatientAsync(dto);
            return result.IsSucceded ? Ok(result) : BadRequest(result);
        }
        #endregion

        #region RegisterHospital

        [HttpPost("register/hospital")]
        public async Task<IActionResult> RegisterHospital([FromBody] HospitalRegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(Response<string>.Fail("Invalid model state."));

            var result = await _authManager.RegisterHospitalAsync(dto);
            return result.IsSucceded ? Ok(result) : BadRequest(result);
        }
        #endregion

        #region Login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _authManager.LoginAsync(dto);
            return result.IsSucceded ? Ok(result) : Unauthorized(result);
        }
        #endregion

        #region Logout
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var result = await _authManager.LogoutAsync(token);
            return result.IsSucceded ? Ok(result) : BadRequest(result);
        }
        #endregion

        #region change password

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(Response<string>.Fail("User not found."));

            var result = await _authManager.ChangePasswordAsync(userId, dto);
            return result.IsSucceded ? Ok(result) : BadRequest(result);
        }
        #endregion
    }
}
