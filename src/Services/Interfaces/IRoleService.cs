using JWTAuthApp.Models;

namespace JWTAuthApp.Services.Interfaces
{
    public interface IRoleService
    {
        Task<ApiResponse> AssignToRoleAsync(Guid userId, AddRoleModel model);
    }
}
