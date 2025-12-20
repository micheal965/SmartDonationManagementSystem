using SmartDonationSystem.Core.Auth.DTOs;
using SmartDonationSystem.Core.Auth.Models;
using SmartDonationSystem.Core.DTOs;
using SmartDonationSystem.Shared.Responses;

namespace SmartDonationSystem.Core.Auth.Interfaces;

public interface IAuthServices
{
    Task<Result<ApplicationUser>> RegisterAsync(RegisterRequestDto request);
    Task<Result<LoginOrRotateTokenResponseDto>> LoginAsync(LoginRequestDto loginRequestDto);
    Task<Result<LoginOrRotateTokenResponseDto>> RotateRefreshTokenAsync(string token);
}
