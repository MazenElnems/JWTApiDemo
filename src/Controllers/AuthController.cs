using JWTAuthApp.Models;
using JWTAuthApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JWTAuthApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterModel model)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _authService.RegisterAsync(model);

            if (!response.IsSuccess)
            {   
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
