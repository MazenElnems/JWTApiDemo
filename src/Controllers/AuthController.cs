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

        [HttpPost("Login")] 
        public async Task<IActionResult> LoginAsync([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var reponse = await _authService.GetTokenAsync(model);

            if(!reponse.IsSuccess)
            {
                return BadRequest(reponse);
            }

            return Ok(reponse);
        }
    }
}
