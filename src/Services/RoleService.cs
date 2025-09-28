using JWTAuthApp.Models;
using JWTAuthApp.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace JWTAuthApp.Services
{
    public class RoleService : IRoleService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public RoleService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ApiResponse> AssignToRoleAsync(Guid userId, AddRoleModel model)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null)
            {
                return ApiResponse.Failure(errors: new List<string> { "User not found!" }, "Invalid User!");
            }

            var role = await _roleManager.FindByNameAsync(model.RoleName);
            if (role is null)
            {
                return ApiResponse.Failure(errors: new List<string> { "Role not found!" }, "Invalid Role!");
            }

            if(await _userManager.IsInRoleAsync(user, model.RoleName))
            {
                return ApiResponse.Failure(errors: new List<string> { "User is already in this role!" }, "Role Assignment Failed!");
            }

            var result = await _userManager.AddToRoleAsync(user, model.RoleName);

            if(!result.Succeeded)
            {
                return ApiResponse.Failure(errors: result.Errors.Select(e => e.Description), "Role Assignment Failed!");
            }

            AddRoleResponse response = new AddRoleResponse
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                RoleName = model.RoleName
            };

            return ApiResponse<AddRoleResponse>.Success(response, "Role assigned successfully!");
        }
    }
}
