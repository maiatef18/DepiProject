using Microsoft.AspNetCore.Mvc;
using Mos3ef.BLL.Manager;
using Mos3ef.BLL.Dtos.Auth;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

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

        [HttpPost("register/patient")]
        public async Task<IActionResult> RegisterPatient([FromBody] PatientRegisterDto dto)
        {
            var result = await _authManager.RegisterPatientAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("register/hospital")]
        public async Task<IActionResult> RegisterHospital([FromBody] HospitalRegisterDto dto)
        {
            var result = await _authManager.RegisterHospitalAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _authManager.LoginAsync(dto);
            if (!result.IsSuccess)
                return Unauthorized(result);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


            var result = await _authManager.ChangePasswordAsync(userId, dto);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
