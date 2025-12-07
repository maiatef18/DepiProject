using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Mos3ef.BLL.Dtos.Auth;
using Mos3ef.DAL.Enums;
using Mos3ef.DAL.Models;
using Mos3ef.DAL.Repository.AuthRepository;
using Mos3ef.DAL.Wapper;
using Mos3ef.Api.Exceptions;
using System.ComponentModel.DataAnnotations;
using ValidationException = Mos3ef.Api.Exceptions.ValidationException;

namespace Mos3ef.BLL.Manager.AuthManager
{
    public class AuthManager : IAuthManager
    {
        private readonly IAuthRepository _authRepository;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        public AuthManager(
            IAuthRepository authRepository,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            IMapper mapper,
            IMemoryCache cache)
        {
            _authRepository = authRepository;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _mapper = mapper;
            _cache = cache;
        }

        #region GenerateJWT
        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            if (user == null)
                throw new BadRequestException("User is null while generating token.");

            var jwtSettings = _configuration.GetSection("Jwt");
            var keyString = jwtSettings["Key"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var duration = jwtSettings["DurationInMinutes"];

            if (string.IsNullOrEmpty(keyString) || string.IsNullOrEmpty(issuer) ||
                string.IsNullOrEmpty(audience) || string.IsNullOrEmpty(duration))
                throw new Exception("JWT configuration is missing or invalid.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim(ClaimTypes.Name, user.UserName ?? ""),
                new Claim("UserType", user.UserType.ToString())
            };

            var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            if (role != null)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(duration)),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        #endregion

        #region RegisterPatient
        public async Task<Response<AuthResponseDto>> RegisterPatientAsync(PatientRegisterDto dto)
        {
            if (dto.Password != dto.ConfirmPassword)
                throw new ValidationException(new List<string> { "Passwords do not match." }); ;

            var existingUser = await _authRepository.GetUserByEmailAsync(dto.Email);
            if (existingUser != null)
                throw new BadRequestException("Email already exists.");

            var user = _mapper.Map<ApplicationUser>(dto);

            var (success, error) = await _authRepository.CreateUserAsync(user, dto.Password);
            if (!success)
                throw new BadRequestException(error);

            await _userManager.AddToRoleAsync(user, "Patient");

            var patient = _mapper.Map<Patient>(dto);
            patient.UserId = user.Id;

            await _authRepository.AddPatientProfileAsync(patient);

            var token = await GenerateJwtToken(user);

            var response = _mapper.Map<AuthResponseDto>(user);
            response.Name = dto.Name;
            response.Token = token;

            return Response<AuthResponseDto>.Success(response, "Patient registered successfully!");
        }
        #endregion

        #region RegisterHospital
        public async Task<Response<AuthResponseDto>> RegisterHospitalAsync(HospitalRegisterDto dto)
        {
            var existingUser = await _authRepository.GetUserByEmailAsync(dto.Email);
            if (existingUser != null)
                throw new BadRequestException("Email already exists.");

            var user = _mapper.Map<ApplicationUser>(dto);

            var (success, error) = await _authRepository.CreateUserAsync(user, dto.Password);
            if (!success)
                throw new BadRequestException(error);

            await _userManager.AddToRoleAsync(user, "Hospital");

            var hospital = _mapper.Map<Hospital>(dto);
            hospital.UserId = user.Id;
            await _authRepository.AddHospitalProfileAsync(hospital);

            var token = await GenerateJwtToken(user);

            var response = _mapper.Map<AuthResponseDto>(user);
            response.Name = dto.Name;
            response.Token = token;

            return Response<AuthResponseDto>.Success(response, "Hospital registered successfully!");
        }
        #endregion

        #region Login
        public async Task<Response<AuthResponseDto>> LoginAsync(LoginDto dto)
        {
            
            var user = await _authRepository.GetUserByEmailAsync(dto.Email);
            if (user == null)
                throw new UnauthorizedException("Invalid login attempt.");

            
            var result = await _signInManager.PasswordSignInAsync(user, dto.Password, false, false);
            if (!result.Succeeded)
                throw new UnauthorizedException("Invalid login attempt.");

            
            var token = await GenerateJwtToken(user);

            
            var response = _mapper.Map<AuthResponseDto>(user);
            response.Token = token;

            
            var cacheKey = $"User_{user.Id}";
            _cache.Set(cacheKey, user,
                new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10)));

            
            return Response<AuthResponseDto>.Success(response, "Login successful!");
        }

        #endregion

        #region Logout
        public async Task<Response<string>> LogoutAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new BadRequestException("Token is required.");

           
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                throw new BadRequestException("Invalid token: UserId not found.");

            
            var expiration = jwtToken.ValidTo;
            await _authRepository.RevokeTokenAsync(token, expiration);

            
            var cacheKey = $"User_{userIdClaim}";
            _cache.Remove(cacheKey);

            return Response<string>.Success(null, "Logged out successfully.");
        }

        #endregion

        #region ChangePassword
        public async Task<Response<string>> ChangePasswordAsync(string userId, ChangePasswordDto dto)
        {
            if (dto.NewPassword != dto.ConfirmNewPassword)
                throw new ValidationException(new List<string> { "Passwords do not match." });

            var user = await _authRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new NotFoundException("User not found.");

            var result = await _authRepository.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            if (!result.IsSuccess)
                throw new BadRequestException(result.Error);

            _cache.Remove($"User_{user.Id}");

            return Response<string>.Success(null, "Password changed successfully.");
        }
        #endregion
    }
}
