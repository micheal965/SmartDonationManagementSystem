using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SmartDonationSystem.Shared.Responses;

namespace SmartDonationSystem.Core.User.Interfaces;

public interface IUserServices
{
    Task<Result<object>> ChangePasswordAsync(string userId, string oldPassword, string newPassword);
    Task<Result<object>> AddOrUpdateProfilePictureAsync(string userId, IFormFile profilePicture);
    Task<Result<object>> DeleteProfilePictureAsync(string userId);
    Task<Result<string>> GetProfilePictureAsync(string userId);
}
