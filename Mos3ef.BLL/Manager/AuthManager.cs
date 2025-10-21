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

namespace Mos3ef.BLL.Manager
{
    public class AuthManager : IAuthManager
    {
        private readonly IAuthRepository _authRepository;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public AuthManager(
            IAuthRepository authRepository,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            IMapper mapper)
        {
            _authRepository = authRepository;
            _signInManager = signInManager;
            _configuration = configuration;
            _mapper = mapper;
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
    new Claim(ClaimTypes.NameIdentifier, user.Id), 
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

            var response = _mapper.Map<AuthResponseDto>(user);

            response.IsSuccess = true;
            response.Message = "Login successful!";
            response.Token = token;

            return response;
        }

        public async Task<AuthResponseDto> RegisterHospitalAsync(HospitalRegisterDto dto)
        {
            var existingUser = await _authRepository.GetUserByEmailAsync(dto.Email);
            if (existingUser != null)
                return new AuthResponseDto { IsSuccess = false, Message = "Email already exists." };

            var user = _mapper.Map<ApplicationUser>(dto);


            var (success, error) = await _authRepository.CreateUserAsync(user, dto.Password);

            if (!success)
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = $"Failed to create Hospital account: {error}"
                };
            }

            var hospital = _mapper.Map<Hospital>(dto);
            hospital.UserId = user.Id;

            await _authRepository.AddHospitalProfileAsync(hospital);

            var token = GenerateJwtToken(user);

            var response = _mapper.Map<AuthResponseDto>(user);

            response.IsSuccess = true;
            response.Message = "Hospital registered successfully!";
            response.Token = token;

            return response;

        }

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
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Email already exists."
                };
            }

            
            var user = _mapper.Map<ApplicationUser>(dto);

           
            user.UserName = new string(dto.Name.Where(char.IsLetterOrDigit).ToArray());
            if (string.IsNullOrEmpty(user.UserName))
                user.UserName = "User" + new Random().Next(1000, 9999);

            
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
                UserId = user.Id,
                Name = dto.Name 
            };

            await _authRepository.AddPatientProfileAsync(patient);

            
            var token = GenerateJwtToken(user);

            
            var response = _mapper.Map<AuthResponseDto>(user);
           
            response.IsSuccess = true;
            response.Message = "Patient registered successfully!";
            response.Token = token;

            return response;
        }

    
    public async Task<AuthResponseDto> ChangePasswordAsync(string userId, ChangePasswordDto dto)
        {
            
            if (dto.NewPassword != dto.ConfirmNewPassword)
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "New password and confirmation do not match."
                };

            
            var user = await _authRepository.GetUserByIdAsync(userId);
            if (user == null)
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "User not found."
                };

           
            var result = await _authRepository.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);

            if (!result.IsSuccess)
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = $"Failed to change password: {result.Error}"
                };

            return new AuthResponseDto
            {
                IsSuccess = true,
                Message = "Password changed successfully."
            };
        }





    }
}
