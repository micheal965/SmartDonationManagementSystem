using Microsoft.AspNetCore.Http;
using SmartDonationSystem.Core.User.DTOs;
using SmartDonationSystem.Shared.Responses;

namespace SmartDonationSystem.Core.User.Interfaces;

public interface IUserServices
{
    Task<Result<object>> ChangePasswordAsync(string userId, string oldPassword, string newPassword);
    Task<Result<object>> AddOrUpdateProfilePictureAsync(string userId, IFormFile profilePicture);
    Task<Result<object>> DeleteProfilePictureAsync(string userId);
    Task<Result<string>> GetProfilePictureAsync(string userId);
    Task<Result<object>> DeleteUserSoftAsync(string userId);
    Task<Result<UserToReturnDto>> GetSpecificUserAsync(string userId);
    Task<Result<object>> UpdateUserAsync(string? userId, UpdateUserRequestDto updateUserRequestDto);

}
