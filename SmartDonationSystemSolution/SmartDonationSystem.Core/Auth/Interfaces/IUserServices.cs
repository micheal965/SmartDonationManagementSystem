using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SmartDonationSystem.Core.Auth.Models;
using SmartDonationSystem.Shared.Responses;

namespace SmartDonationSystem.Core.Auth.Interfaces;

public interface IUserServices
{
    Task<Result<object>> ChangePasswordAsync(string userId, string oldPassword, string newPassword);
    Task<Result<IdentityResult>> AddOrUpdateProfilePictureAsync(string userId, IFormFile profilePicture);
    Task<Result<object>> DeleteProfilePictureAsync(string userId);
    Task<Result<string>> GetProfilePictureAsync(string userId);
}
