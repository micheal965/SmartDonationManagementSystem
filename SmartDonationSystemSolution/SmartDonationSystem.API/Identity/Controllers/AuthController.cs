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
            await _authServices.RegisterAsync(registerRequestDto);
            return Ok();
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginDto)
        {
            var loginResponse = await _authServices.LoginAsync(loginDto);
            return StatusCode((int)loginResponse.statusCode);
        }
        [HttpPost("rotate-refresh-token")]
        public async Task<IActionResult> RotateRefreshToken()
        {
            var token = Request.Cookies["refreshToken"];
            var result = await _authServices.RotateRefreshTokenAsync(token);
            return StatusCode((int)result.statusCode);
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

    }
}
