using JWTAuthApp.Models;
using JWTAuthApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JWTAuthApp.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = RoleNames.Admin)]
    [ApiController]
    public class UsersController : ControllerBase 
    {
        private readonly IRoleService _roleService;
        private readonly IAuthService _authService;

        public UsersController(IRoleService roleService, IAuthService authService)
        {
            _roleService = roleService;
            _authService = authService;
        }

        [HttpPost("roles/{UserId}")]
        public async Task<IActionResult> AddToRoleAsync(Guid UserId,[FromBody] AddRoleModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _roleService.AssignToRoleAsync(UserId, model);

            if(!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("Register/{Role}")]
        public async Task<IActionResult> CreateAsync(string Role,[FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _authService.RegisterAsync(Role, model);
            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }

            var Data = new
            {
                ((ApiResponse<AuthResponse>)response).Data.UserName,
                ((ApiResponse<AuthResponse>)response).Data.Email,
                ((ApiResponse<AuthResponse>)response).Data.Id
            };

            return Ok(new
            {
                response.Message,
                response.IsSuccess,
                Data
            });
        }
    }
}
