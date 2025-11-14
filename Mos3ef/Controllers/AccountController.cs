using Microsoft.AspNetCore.Mvc;
using Mos3ef.BLL.Dtos.Auth;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using Mos3ef.DAL.Models;
using Mos3ef.BLL.Manager.AuthManager;

namespace Mos3ef.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthManager _authManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public AccountController(IAuthManager authManager,
                                 UserManager<ApplicationUser> userManager,
                                 RoleManager<IdentityRole> roleManager)
        {
            _authManager = authManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        #region RegisterPatient
        [HttpPost("register/patient")]
        public async Task<IActionResult> RegisterPatient([FromBody] PatientRegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authManager.RegisterPatientAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }
        #endregion

        #region RegisterHospital
        [HttpPost("register/hospital")]
        public async Task<IActionResult> RegisterHospital([FromBody] HospitalRegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authManager.RegisterHospitalAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }
        #endregion

        #region Login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _authManager.LoginAsync(dto);
            if (!result.IsSuccess)
                return Unauthorized(result);
            return Ok(result);
        }
        #endregion

        #region ChangePassword
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
        #endregion

        #region Roles
        [Authorize(Roles = "Admin")]
        [HttpPost("create-role")]
        public async Task<IActionResult> CreateRole([FromBody] string roleName)
        {
            var result = await _authManager.CreateRoleAsync(roleName);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto dto)
        {
            var result = await _authManager.AssignRoleAsync(dto.Email, dto.RoleName);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        #endregion

        #region User Management

        [Authorize(Roles = "Admin")]
        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            var result = await _authManager.CreateUserAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("update-user")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto dto)
        {
            var result = await _authManager.UpdateUserAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }
        #endregion






    }
}
