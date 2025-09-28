using JWTAuthApp.Models;

namespace JWTAuthApp.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse> RegisterAsync(RegisterModel model);
    }
}
