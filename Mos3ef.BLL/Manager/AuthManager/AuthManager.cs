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

        #region RegisterPatient
        public async Task<Response<AuthResponseDto>> RegisterPatientAsync(PatientRegisterDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Name) ||
                    string.IsNullOrWhiteSpace(dto.Email) ||
                    string.IsNullOrWhiteSpace(dto.Password) ||
                    string.IsNullOrWhiteSpace(dto.ConfirmPassword))
                {
                    return Response<AuthResponseDto>.Fail("All fields are required.");
                }

                if (dto.Password != dto.ConfirmPassword)
                    return Response<AuthResponseDto>.Fail("Password and Confirm Password do not match.");

                var existingUser = await _authRepository.GetUserByEmailAsync(dto.Email);
                if (existingUser != null)
                    return Response<AuthResponseDto>.Fail("Email already exists.");

                var user = _mapper.Map<ApplicationUser>(dto);
                var (success, error) = await _authRepository.CreateUserAsync(user, dto.Password);
                if (!success)
                    return Response<AuthResponseDto>.Fail($"Failed to create patient: {error}");

                if (!await _roleManager.RoleExistsAsync("Patient"))
                    await _roleManager.CreateAsync(new IdentityRole("Patient"));

                await _userManager.AddToRoleAsync(user, "Patient");

                var patient = _mapper.Map<Patient>(dto);
                patient.UserId = user.Id;
                await _authRepository.AddPatientProfileAsync(patient);

                var token = await GenerateJwtToken(user);
                var response = _mapper.Map<AuthResponseDto>(user);
                response.Name = dto.Name;
                response.Token = token;

                _cache.Remove($"User_{dto.Email}");

                return Response<AuthResponseDto>.Success(response, "Patient registered successfully!");
            }
            catch (Exception ex)
            {
                return Response<AuthResponseDto>.Fail($"Registration failed: {ex.Message}");
            }
        }
        #endregion

        #region RegisterHospital
        public async Task<Response<AuthResponseDto>> RegisterHospitalAsync(HospitalRegisterDto dto)
        {
            try
            {
                var existingUser = await _authRepository.GetUserByEmailAsync(dto.Email);
                if (existingUser != null)
                    return Response<AuthResponseDto>.Fail("Email already exists.");

                var user = _mapper.Map<ApplicationUser>(dto);
                var (success, error) = await _authRepository.CreateUserAsync(user, dto.Password);
                if (!success)
                    return Response<AuthResponseDto>.Fail($"Failed to create hospital: {error}");

                if (!await _roleManager.RoleExistsAsync("Hospital"))
                    await _roleManager.CreateAsync(new IdentityRole("Hospital"));

                await _userManager.AddToRoleAsync(user, "Hospital");

                var token = await GenerateJwtToken(user);
                var response = _mapper.Map<AuthResponseDto>(user);
                response.Name = dto.Name;
                response.Token = token;

                var hospital = _mapper.Map<Hospital>(dto);
                hospital.UserId = user.Id;
                await _authRepository.AddHospitalProfileAsync(hospital);


                return Response<AuthResponseDto>.Success(response, "Hospital registered successfully!");
            }
            catch (Exception ex)
            {
                return Response<AuthResponseDto>.Fail($"Registration failed: {ex.Message}");
            }
        }
        #endregion

        #region Login
        public async Task<Response<AuthResponseDto>> LoginAsync(LoginDto dto)
        {
            try
            {

                var cacheKey = $"User_{dto.Email}";
                if (!_cache.TryGetValue(cacheKey, out ApplicationUser user))
                {
                    user = await _authRepository.GetUserByEmailAsync(dto.Email);
                    if (user == null)
                        return Response<AuthResponseDto>.Fail("Invalid login attempt.");

                    _cache.Set(cacheKey, user, new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(5)));
                }

                var result = await _signInManager.PasswordSignInAsync(user, dto.Password, false, false);
                if (!result.Succeeded)
                    return Response<AuthResponseDto>.Fail("Invalid login attempt.");

                var token = await GenerateJwtToken(user);
                var response = _mapper.Map<AuthResponseDto>(user);
                response.Token = token;

                return Response<AuthResponseDto>.Success(response, "Login successful!");
            }
            catch (Exception ex)
            {
                return Response<AuthResponseDto>.Fail($"Login failed: {ex.Message}");
            }
        }
        #endregion

        #region Logout
        public async Task<Response<string>> LogoutAsync(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                    return Response<string>.Fail("Token is missing.");

                var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
                var expiration = jwtToken.ValidTo;

                await _authRepository.RevokeTokenAsync(token, expiration);

                return Response<string>.Success(null, "Logged out successfully.");
            }
            catch (Exception ex)
            {
                return Response<string>.Fail($"Logout failed: {ex.Message}");
            }
        }
        #endregion

        #region ChangePassword
        public async Task<Response<string>> ChangePasswordAsync(string userId, ChangePasswordDto dto)
        {
            try
            {
                if (dto.NewPassword != dto.ConfirmNewPassword)
                    return Response<string>.Fail("New password and confirmation do not match.");

                var user = await _authRepository.GetUserByIdAsync(userId);
                if (user == null)
                    return Response<string>.Fail("User not found.");

                var result = await _authRepository.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
                if (!result.IsSuccess)
                    return Response<string>.Fail($"Failed to change password: {result.Error}");

                _cache.Remove($"User_{user.Email}");

                return Response<string>.Success(null, "Password changed successfully.");
            }
            catch (Exception ex)
            {
                return Response<string>.Fail($"Change password failed: {ex.Message}");
            }
        }
        #endregion
    }
}
