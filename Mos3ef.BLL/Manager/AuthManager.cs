using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Mos3ef.BLL.Dtos.Auth;
using Mos3ef.DAL.Enums;
using Mos3ef.DAL.Models;
using Mos3ef.DAL.Repository;

namespace Mos3ef.BLL.Manager
{
    public class AuthManager : IAuthManager
    {
        private readonly IAuthRepository _authRepository;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthManager(IAuthRepository authRepository,
                         SignInManager<ApplicationUser> signInManager,
                         IConfiguration configuration)
        {
            _authRepository = authRepository;
            _signInManager = signInManager;
            _configuration = configuration;
        }
        private string GenerateJwtToken(ApplicationUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var jwtSettings = _configuration.GetSection("Jwt");

            string keyString = jwtSettings["Key"];
            string issuer = jwtSettings["Issuer"];
            string audience = jwtSettings["Audience"];
            string duration = jwtSettings["DurationInMinutes"];

            if (string.IsNullOrEmpty(keyString) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience) || string.IsNullOrEmpty(duration))
                throw new Exception("JWT configuration is missing or invalid.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));

            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id),
        new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
        new Claim("userType", user.UserType.ToString())
    };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(duration)),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _authRepository.GetUserByEmailAsync(dto.Email);
            if (user == null)
                return new AuthResponseDto { IsSuccess = false, Message = "Invalid login attempt." };

            var result = await _signInManager.PasswordSignInAsync(user, dto.Password, false, false);
            if (!result.Succeeded)
                return new AuthResponseDto { IsSuccess = false, Message = "Invalid login attempt." };

            var token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                IsSuccess = true,
                Message = "Login successful!",
                Token = token,
                UserId = user.Id,
                Email = user.Email,
                Name = user.UserType == UserType.Patient ? user.PatientProfile?.User?.UserName : user.HospitalProfile?.Name,
                UserType = user.UserType,
                PatientProfileId = user.PatientProfile?.PatientId,
                HospitalProfileId = user.HospitalProfile?.HospitalId
            };
        }

        public async Task<AuthResponseDto> RegisterHospitalAsync(HospitalRegisterDto dto)
        {
            var existingUser = await _authRepository.GetUserByEmailAsync(dto.Email);
            if (existingUser != null)
                return new AuthResponseDto { IsSuccess = false, Message = "Email already exists." };

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                UserType = UserType.Hospital
            };

            var (success, error) = await _authRepository.CreateUserAsync(user, dto.Password);

            if (!success)
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = $"Failed to create Hospital account: {error}"
                };
            }

            var hospital = new Hospital
            {
                Name = dto.Name,
                Address = dto.Address,
                Location = dto.Location,
                Phone_Number = dto.PhoneNumber,
                UserId = user.Id
            };

            await _authRepository.AddHospitalProfileAsync(hospital);

            var token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                IsSuccess = true,
                Message = "Hospital registered successfully!",
                Token = token,
                UserId = user.Id,
                Email = user.Email,
                Name = hospital.Name,
                UserType = user.UserType,
                HospitalProfileId = hospital.HospitalId
            };
        }

        public async Task<AuthResponseDto> RegisterPatientAsync(PatientRegisterDto dto)
        {
           
            if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Email)
                || string.IsNullOrWhiteSpace(dto.Password) || string.IsNullOrWhiteSpace(dto.ConfirmPassword))
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Name, Email, Password, and ConfirmPassword are required."
                };
            }

            
            var existingUser = await _authRepository.GetUserByEmailAsync(dto.Email);
            if (existingUser != null)
                return new AuthResponseDto { IsSuccess = false, Message = "Email already exists." };

            string username = new string(dto.Name.Where(char.IsLetterOrDigit).ToArray());
            if (string.IsNullOrEmpty(username))
                username = "User" + new Random().Next(1000, 9999);

            
            var user = new ApplicationUser
            {
                UserName = username, 
                Email = dto.Email,
                UserType = UserType.Patient
            };

            
            var (success, error) = await _authRepository.CreateUserAsync(user, dto.Password);
            if (!success)
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = $"Failed to create patient account: {error}"
                };
            }

            
            var patient = new Patient
            {
                Name=user.UserName,
                UserId = user.Id,
                Location = dto.Location,
                Address = dto.Address
            };
            await _authRepository.AddPatientProfileAsync(patient);

            
            var token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                IsSuccess = true,
                Message = "Patient registered successfully!",
                Token = token,
                UserId = user.Id,
                Email = user.Email,
                Name = dto.Name,
                UserType = user.UserType,
                PatientProfileId = patient.PatientId
            };
        }




    }
}
