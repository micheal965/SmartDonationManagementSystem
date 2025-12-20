using System;
using SmartDonationSystem.Core.Auth.Models;
using SmartDonationSystem.Shared.Responses;

namespace SmartDonationSystem.Core.Auth.Interfaces;

public interface IUserServices
{
    Task SaveLoginAttemptAsync(string IdentityNumber);
    Task<Result<IReadOnlyList<UserLoginHistory>>> GetLoginHistoryAsync(string userId);

    // Task<IdentityResult> AddOrUpdateProfilePictureAsync(string userId, IFormFile profilePicture);
    // Task<bool> DeleteProfilePictureAsync(string userId);
    // Task<string> GetProfilePictureAsync(string userId);

}
