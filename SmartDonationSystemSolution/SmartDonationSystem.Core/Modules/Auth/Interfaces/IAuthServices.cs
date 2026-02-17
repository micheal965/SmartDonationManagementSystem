using SmartDonationSystem.Core.Modules.Auth.DTOs;
using SmartDonationSystem.Shared.Responses;

namespace SmartDonationSystem.Core.Modules.Auth.Interfaces;

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
