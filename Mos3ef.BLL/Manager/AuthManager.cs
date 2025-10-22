using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Mos3ef.BLL.Dtos.Auth;
using Mos3ef.DAL.Enums;
using Mos3ef.DAL.Models;
using Mos3ef.DAL.Repository;
using  Mos3ef.BLL.Dtos.Auth;
using Microsoft.EntityFrameworkCore;

namespace Mos3ef.BLL.Manager
{
    public class AuthManager : IAuthManager
    {
        private readonly IAuthRepository _authRepository;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public AuthManager(
            IAuthRepository authRepository,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            IMapper mapper)
        {
            _authRepository = authRepository;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _mapper = mapper;
        }

        #region GenerateJWTToken
        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var jwtSettings = _configuration.GetSection("Jwt");

            string keyString = jwtSettings["Key"];
            string issuer = jwtSettings["Issuer"];
            string audience = jwtSettings["Audience"];
            string duration = jwtSettings["DurationInMinutes"];

            if (string.IsNullOrEmpty(keyString) ||
                string.IsNullOrEmpty(issuer) ||
                string.IsNullOrEmpty(audience) ||
                string.IsNullOrEmpty(duration))
                throw new Exception("JWT configuration is missing or invalid.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));


            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Email, user.Email ?? ""),
        new Claim(ClaimTypes.Name, user.UserName ?? ""),
        new Claim("UserType", user.UserType.ToString())
    };


            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }


            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(duration)),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        #endregion

        #region login
        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _authRepository.GetUserByEmailAsync(dto.Email);
            if (user == null)
                return new AuthResponseDto { IsSuccess = false, Message = "Invalid login attempt." };

            var result = await _signInManager.PasswordSignInAsync(user, dto.Password, false, false);
            if (!result.Succeeded)
                return new AuthResponseDto { IsSuccess = false, Message = "Invalid login attempt." };

            var token = await GenerateJwtToken(user);


            var response = _mapper.Map<AuthResponseDto>(user);

            response.IsSuccess = true;
            response.Message = "Login successful!";
            response.Token = token;

            return response;
        }
        #endregion

        #region RegisterHospital

        public async Task<AuthResponseDto> RegisterHospitalAsync(HospitalRegisterDto dto)
        {
            var existingUser = await _authRepository.GetUserByEmailAsync(dto.Email);
            if (existingUser != null)
                return new AuthResponseDto { IsSuccess = false, Message = "Email already exists." };


            var user = _mapper.Map<ApplicationUser>(dto);


            var (success, error) = await _authRepository.CreateUserAsync(user, dto.Password);
            if (!success)
                return new AuthResponseDto { IsSuccess = false, Message = $"Failed to create hospital: {error}" };


            if (!await _roleManager.RoleExistsAsync("Hospital"))
                await _roleManager.CreateAsync(new IdentityRole("Hospital"));

            await _userManager.AddToRoleAsync(user, "Hospital");


            var hospital = _mapper.Map<Hospital>(dto);
            hospital.UserId = user.Id;
            await _authRepository.AddHospitalProfileAsync(hospital);


            var token = await GenerateJwtToken(user);


            var response = _mapper.Map<AuthResponseDto>(user);
            response.IsSuccess = true;
            response.Message = "Hospital registered successfully!";
            response.Token = token;

            return response;
        }
        #endregion

        #region RegisterPatient
        public async Task<AuthResponseDto> RegisterPatientAsync(PatientRegisterDto dto)
        {
            
            if (string.IsNullOrWhiteSpace(dto.Name) ||
                string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Password) ||
                string.IsNullOrWhiteSpace(dto.ConfirmPassword))
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Name, Email, Password, and ConfirmPassword are required."
                };
            }

            if (dto.Password != dto.ConfirmPassword)
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Password and Confirm Password do not match."
                };
            }


            var existingUser = await _authRepository.GetUserByEmailAsync(dto.Email);
            if (existingUser != null)
                return new AuthResponseDto { IsSuccess = false, Message = "Email already exists." };


            var user = _mapper.Map<ApplicationUser>(dto);


            var (success, error) = await _authRepository.CreateUserAsync(user, dto.Password);
            if (!success)
                return new AuthResponseDto { IsSuccess = false, Message = $"Failed to create patient: {error}" };


            if (!await _roleManager.RoleExistsAsync("Patient"))
                await _roleManager.CreateAsync(new IdentityRole("Patient"));

            await _userManager.AddToRoleAsync(user, "Patient");


            var patient = _mapper.Map<Patient>(dto);
            patient.UserId = user.Id;
            await _authRepository.AddPatientProfileAsync(patient);


            var token = await GenerateJwtToken(user);


            var response = _mapper.Map<AuthResponseDto>(user);
            response.IsSuccess = true;
            response.Message = "Patient registered successfully!";
            response.Token = token;

            return response;
        }
        #endregion

        #region ChangePassword

        public async Task<BasicResponseDto> ChangePasswordAsync(string userId, ChangePasswordDto dto)
        {

            if (dto.NewPassword != dto.ConfirmNewPassword)
                return new BasicResponseDto
                {
                    IsSuccess = false,
                    Message = "New password and confirmation do not match."
                };


            var user = await _authRepository.GetUserByIdAsync(userId);
            if (user == null)
                return new BasicResponseDto
                {
                    IsSuccess = false,
                    Message = "User not found."
                };


            var result = await _authRepository.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);

            if (!result.IsSuccess)
                return new BasicResponseDto
                {
                    IsSuccess = false,
                    Message = $"Failed to change password: {result.Error}"
                };

            return new BasicResponseDto
            {
                IsSuccess = true,
                Message = "Password changed successfully."
            };
        }
        #endregion

        #region AssignRole
        public async Task<BasicResponseDto> AssignRoleAsync(string email, string roleName)
        {
            var response = new BasicResponseDto();

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(roleName))
            {
                response.IsSuccess = false;
                response.Message = "Email and RoleName are required.";
                return response;
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                response.IsSuccess = false;
                response.Message = $"User with email '{email}' not found.";
                return response;
            }

            if (!await _roleManager.RoleExistsAsync(roleName))
                await _roleManager.CreateAsync(new IdentityRole(roleName));

            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles.Contains(roleName))
            {
                response.IsSuccess = false;
                response.Message = $"User already has the '{roleName}' role.";
                return response;
            }
            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (!result.Succeeded)
            {
                response.IsSuccess = false;
                response.Message = $"Failed to assign role: {string.Join(", ", result.Errors.Select(e => e.Description))}";
                return response;
            }

            response.IsSuccess = true;
            response.Message = $"Role '{roleName}' assigned to user '{email}' successfully.";
            return response;
        }
        #endregion

        #region CreateRole

        public async Task<BasicResponseDto> CreateRoleAsync(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                return new BasicResponseDto
                {
                    IsSuccess = false,
                    Message = "Role name cannot be empty."
                };
            }

            
            if (await _roleManager.RoleExistsAsync(roleName))
            {
                return new BasicResponseDto
                {
                    IsSuccess = false,
                    Message = $"Role '{roleName}' already exists."
                };
            }

            
            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new BasicResponseDto
                {
                    IsSuccess = false,
                    Message = $"Failed to create role: {errors}"
                };
            }

            return new BasicResponseDto
            {
                IsSuccess = true,
                Message = $"Role '{roleName}' created successfully."
            };
        }
        #endregion

        #region CreateUser

        public async Task<BasicResponseDto> CreateUserAsync(CreateUserDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return new BasicResponseDto { IsSuccess = false, Message = "Email and Password are required." };

            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                return new BasicResponseDto { IsSuccess = false, Message = "User with this email already exists." };

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                UserType = Enum.TryParse<UserType>(dto.Role, out var type) ? type : UserType.Patient
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return new BasicResponseDto
                {
                    IsSuccess = false,
                    Message = string.Join(", ", result.Errors.Select(e => e.Description))
                };

            await AssignRoleAsync(dto.Email, dto.Role);

            return new BasicResponseDto
            {
                IsSuccess = true,
                Message = $"User '{dto.Email}' created successfully with role '{dto.Role}'."
            };
        }
        #endregion

        #region UpdateUser
        public async Task<BasicResponseDto> UpdateUserAsync(UpdateUserDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return new BasicResponseDto { IsSuccess = false, Message = "User not found." };

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return new BasicResponseDto
                {
                    IsSuccess = false,
                    Message = string.Join(", ", updateResult.Errors.Select(e => e.Description))
                };

            if (!string.IsNullOrWhiteSpace(dto.NewPassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordResult = await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);

                if (!passwordResult.Succeeded)
                    return new BasicResponseDto
                    {
                        IsSuccess = false,
                        Message = string.Join(", ", passwordResult.Errors.Select(e => e.Description))
                    };
            }

            return new BasicResponseDto
            {
                IsSuccess = true,
                Message = $"User '{dto.Email}' updated successfully."
            };
        }
        #endregion

    }
}
