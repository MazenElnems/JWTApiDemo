using JWTAuthApp.Models;
using JWTAuthApp.Models.Entities;
using JWTAuthApp.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace JWTAuthApp.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly JWT _jwt;

        public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager, IOptions<JWT> jwt)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwt = jwt.Value;
        }

        public async Task<ApiResponse> GetTokenAsync(LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if(user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return ApiResponse.Failure(errors: new List<string> { "Invalid Email or Password!" }, "Login Failed!");
            }

            RefreshToken refreshToken = new RefreshToken
            {
                CreatedAt = DateTime.UtcNow,
                ExpiresOn = DateTime.UtcNow.AddDays(30),
                Token = GenerateRefreshToken()
            };

            user.RefreshTokens.Add(refreshToken);
            user.RefreshTokens.RemoveAll(rt => !rt.IsActive);
            await _userManager.UpdateAsync(user);

            var authResponse = new AuthResponse
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                AccessTokenExpiration = DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes),
                Token = await GenerateJwtToken(user),
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiration = refreshToken.ExpiresOn
            };

            return ApiResponse<AuthResponse>.Success(authResponse, "Login Successful!");
        }

        public async Task<ApiResponse> RefreshTokenAsync(string token)
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token ));
        
            if(user is null)
            {
                return ApiResponse.Failure(new List<string> { "Invalid refresh token" });
            }

            var oldRefreshToken = user.RefreshTokens.FirstOrDefault(t => t.Token == token && t.IsActive);

            if(oldRefreshToken is null)
            {
                return ApiResponse.Failure(new List<string> { "Inactive refresh token" });
            }

            oldRefreshToken.RevokedOn = DateTime.UtcNow;

            var newRefreshToken = new RefreshToken
            {
                Token = GenerateRefreshToken(),
                CreatedAt = DateTime.UtcNow,
                ExpiresOn = oldRefreshToken.ExpiresOn
            };

            user.RefreshTokens.Add(newRefreshToken);
            await _userManager.UpdateAsync(user);

            var authResponse = new AuthResponse
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Token = await GenerateJwtToken(user),
                RefreshTokenExpiration = newRefreshToken.ExpiresOn,
                AccessTokenExpiration = DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes),
                RefreshToken = newRefreshToken.Token,
            };

            return ApiResponse<AuthResponse>.Success(authResponse, "get access token successfully!");
        }

        public async Task<ApiResponse> RegisterAsync(RegisterModel model)
        {
            return await RegisterAsync(RoleNames.User, model);
        }

        public async Task<ApiResponse> RegisterAsync(string role, RegisterModel model)
        {
            if (await _userManager.FindByNameAsync(model.UserName) is not null)
            {
                return ApiResponse.Failure(errors: new List<string> { "Username is already taken!" }, "Invalid Username!");
            }

            if (await _userManager.FindByEmailAsync(model.Email) is not null)
            {
                return ApiResponse.Failure(errors: new List<string> { "Email is already taken!" }, "Invalid Email!");
            }

            if(!await _roleManager.RoleExistsAsync(role))
            {
                return ApiResponse.Failure(errors: new List<string> { "Role does not exist!" }, "Invalid Role!");
            }

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return ApiResponse.Failure(errors: result.Errors.Select(e => e.Description), "User creation failed!");
            }

            result = await _userManager.AddToRoleAsync(user, role);

            if (!result.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                return ApiResponse.Failure(errors: result.Errors.Select(e => e.Description), "Assigning role to user failed!");
            }

            RefreshToken refreshToken = new RefreshToken
            {
                CreatedAt = DateTime.UtcNow,
                ExpiresOn = DateTime.UtcNow.AddDays(30),
                Token = GenerateRefreshToken()
            };

            user.RefreshTokens.Add(refreshToken);
            await _userManager.UpdateAsync(user);

            AuthResponse authResponse = new AuthResponse
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                AccessTokenExpiration = DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes),
                Token = await GenerateJwtToken(user),
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiration = refreshToken.ExpiresOn
            };

            return ApiResponse<AuthResponse>.Success(authResponse, "User created successfully!");
        }

        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = roles.Select(role => new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id.ToString())
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes),
                signingCredentials: signingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
