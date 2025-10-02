using JWTAuthApp.Models;

namespace JWTAuthApp.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse> RegisterAsync(RegisterModel model);
        Task<ApiResponse> RegisterAsync(string role,RegisterModel model);
        Task<ApiResponse> GetTokenAsync(LoginModel model);
        Task<ApiResponse> RefreshTokenAsync(string token);
    }
}
