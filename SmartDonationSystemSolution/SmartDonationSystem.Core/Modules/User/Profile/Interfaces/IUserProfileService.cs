using Microsoft.AspNetCore.Http;
using SmartDonationSystem.Core.Modules.Auth.DTOs;
using SmartDonationSystem.Core.Modules.User.Profile.DTOs;
using SmartDonationSystem.Shared.Responses;

namespace SmartDonationSystem.Core.Modules.User.Profile.Interfaces;

public interface IUserProfileService
{
    Task<Result<object>> ChangePasswordAsync(string userId, string oldPassword, string newPassword);
    Task<Result<object>> AddOrUpdateProfilePictureAsync(string userId, IFormFile profilePicture);
    Task<Result<object>> DeleteProfilePictureAsync(string userId);
    Task<Result<string>> GetProfilePictureAsync(string userId);
    Task<Result<object>> DeleteUserSoftAsync(string userId);
    Task<Result<UserToReturnDto>> GetSpecificUserAsync(string userId);
    Task<Result<object>> UpdateUserAsync(string? userId, UpdateUserRequestDto updateUserRequestDto);
    Task<Result<IReadOnlyList<UserLoginsHistoryResponseDto>>> GetLoginHistoryAsync(string userId);
}
