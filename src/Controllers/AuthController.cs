using Azure;
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

            var authresponse = (response as ApiResponse<AuthResponse>)!.Data;
            SetRefreshToken(authresponse.RefreshToken, authresponse.RefreshTokenExpiration);
            return Ok(response);
        }

        [HttpPost("Token")] 
        public async Task<IActionResult> LoginAsync([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _authService.GetTokenAsync(model);

            if(!response.IsSuccess)
            {
                return BadRequest(response);
            }

            var authresponse = (response as ApiResponse<AuthResponse>)!.Data;
            SetRefreshToken(authresponse.RefreshToken, authresponse.RefreshTokenExpiration);
            return Ok(response);
        }

        [HttpGet("refresh-token")]
        public async Task<IActionResult> RefreshTokenAsync()
        {
            var token = Request.Cookies["refresh-token"];

            if (token == null)
                return BadRequest(new
                {
                    Message = "there is no refreshtoken"
                });

            var response = await _authService.RefreshTokenAsync(token);

            if(!response.IsSuccess)
                return BadRequest(response);

            var authresponse = (response as ApiResponse<AuthResponse>)!.Data;
            SetRefreshToken(authresponse.RefreshToken, authresponse.RefreshTokenExpiration);
            return Ok(response);
        }

        private void SetRefreshToken(string refreshToken,DateTime expires,string? path = null)
        {
            Response.Cookies.Append("refresh-token", refreshToken, new CookieOptions
            {
                Path = path,
                Expires = expires.ToLocalTime(),
                HttpOnly = true
            });
        } 
    }
}
