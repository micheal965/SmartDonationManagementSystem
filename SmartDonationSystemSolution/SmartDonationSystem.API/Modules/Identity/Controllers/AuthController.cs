using Microsoft.AspNetCore.Mvc;
using SmartDonationSystem.Core.Modules.Auth.DTOs;
using SmartDonationSystem.Core.Modules.Auth.Interfaces;
using SmartDonationSystem.Shared.Responses;

namespace SmartDonationSystem.API.Modules.Identity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authServices;

        public AuthController(IAuthService authServices)
        {
            _authServices = authServices;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDto registerRequestDto)
        {
            var registerResponse = await _authServices.RegisterAsync(registerRequestDto);
            return StatusCode((int)registerResponse.statusCode, registerResponse);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginDto)
        {
            var loginResponse = await _authServices.LoginAsync(loginDto);
            return StatusCode((int)loginResponse.statusCode, loginResponse);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromHeader] string Authorization)
        {
            if (string.IsNullOrWhiteSpace(Authorization) || !Authorization.StartsWith("Bearer "))
                return BadRequest(Result<string>.BadRequest("Invalid Authorization header"));

            var jwtToken = Authorization["Bearer ".Length..].Trim();
            if (string.IsNullOrEmpty(jwtToken))
                return Unauthorized(Result<string>.Unauthorized("Token is missing or invalid"));

            await _authServices.AddTokenBlacklistAsync(jwtToken);
            return Ok(Result<string>.Ok("Logged out successfully"));
        }

        [HttpPost("rotate-refresh-token")]
        public async Task<IActionResult> RotateRefreshToken(RefreshRequestDto request)
        {
            var token = request.refreshToken ?? Request.Cookies["refreshToken"];
            var result = await _authServices.RotateRefreshTokenAsync(token);
            return StatusCode((int)result.statusCode, result);
        }
    }
}
