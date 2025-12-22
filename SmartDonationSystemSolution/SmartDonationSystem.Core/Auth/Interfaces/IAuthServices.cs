using SmartDonationSystem.Core.Auth.DTOs;
using SmartDonationSystem.Core.Auth.Models;
using SmartDonationSystem.Core.DTOs;
using SmartDonationSystem.Shared.Responses;

namespace SmartDonationSystem.Core.Auth.Interfaces;

public interface IAuthServices
{
    Task<Result<RegisterResultDto>> RegisterAsync(RegisterRequestDto request);
    Task<Result<LoginOrRotateTokenResponseDto>> LoginAsync(LoginRequestDto loginRequestDto);
    Task<Result<LoginOrRotateTokenResponseDto>> RotateRefreshTokenAsync(string? token);
    Task SaveLoginAttemptAsync(string IdentityNumber);
    Task<Result<IReadOnlyList<UserLoginsHistoryResponseDto>>> GetLoginHistoryAsync(string userId);
    Task AddTokenBlacklistAsync(string token);
    Task<bool> IsTokenBlacklistedAsync(string token);
}
