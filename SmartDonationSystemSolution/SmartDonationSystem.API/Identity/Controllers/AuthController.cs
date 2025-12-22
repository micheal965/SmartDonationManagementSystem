using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartDonationSystem.Core.Auth.DTOs;
using SmartDonationSystem.Core.Auth.Interfaces;
using SmartDonationSystem.Core.DTOs;
using SmartDonationSystem.Shared.Responses;

namespace SmartDonationSystem.API.Identity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices _authServices;
        private readonly IAuthOcrService _authOcrService;

        public AuthController(IAuthServices authServices, IAuthOcrService authOcrService)
        {
            _authServices = authServices;
            _authOcrService = authOcrService;
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

        [Authorize]
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
        public async Task<IActionResult> RotateRefreshToken()
        {
            var token = Request.Cookies["refreshToken"];
            var result = await _authServices.RotateRefreshTokenAsync(token);
            return StatusCode((int)result.statusCode, result);
        }

        [HttpPost("extract")]
        public async Task<IActionResult> ExtractText(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            try
            {
                return Ok(await _authOcrService.ExtractAsync(file));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "OCR failed: " + ex.Message);
            }

        }

        [Authorize]
        [HttpGet("login-history")]
        public async Task<IActionResult> GetLoginHistory()
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var historyListResult = await _authServices.GetLoginHistoryAsync(userId);
            return StatusCode((int)historyListResult.statusCode, historyListResult);
        }
    }
}
