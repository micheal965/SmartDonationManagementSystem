using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartDonationSystem.Core.Auth.Interfaces;
using SmartDonationSystem.Core.Auth.Models;
using SmartDonationSystem.Core.Cloud;
using SmartDonationSystem.DataAccess;
using SmartDonationSystem.Shared.Responses;

namespace SmartDonationSystem.Services.Identity;

public class UserServices : IUserServices
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly ICloudinaryServices _cloudinaryServices;

    public UserServices(UserManager<ApplicationUser> userManager,
                        ApplicationDbContext applicationDbContext,
                        ICloudinaryServices cloudinaryServices)
    {
        _userManager = userManager;
        _applicationDbContext = applicationDbContext;
        _cloudinaryServices = cloudinaryServices;
    }

    public async Task<Result<object>> ChangePasswordAsync(string userId, string oldPassword, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return Result<object>.BadRequest("Cannot Change Password!");

        var passwordChangeResult = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
        if (!passwordChangeResult.Succeeded)
            return Result<object>.BadRequest($"Password change failed", passwordChangeResult.Errors);

        //forces re-login on other devices.
        await _userManager.UpdateSecurityStampAsync(user);
        return Result<object>.Ok("Password changed successfully");
    }
    public async Task<Result<IdentityResult>> AddOrUpdateProfilePictureAsync(string userId, IFormFile profilePicture)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return Result<IdentityResult>.BadRequest("User not found");

        var uploadResult = await _cloudinaryServices.UploadImageAsync(profilePicture);
        if (!uploadResult.isSucceded)
            return Result<IdentityResult>.BadRequest("Failed to upload profile picture");

        var deleteResult = await _cloudinaryServices.DeleteImageAsync(user.PictureUrl);
        if (!deleteResult)
            return Result<IdentityResult>.BadRequest("Failed to upload profile picture");

        user.PictureUrl = uploadResult.url;
        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
            return Result<IdentityResult>.BadRequest("Failed to update user profile", updateResult.Errors);

        return Result<IdentityResult>.Ok(updateResult, "Profile picture updated successfully");
    }
    public async Task<Result<string>> GetProfilePictureAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return Result<string>.BadRequest("User not found");

        if (string.IsNullOrEmpty(user.PictureUrl))
            return Result<string>.Ok(string.Empty, "No profile picture set");

        return Result<string>.Ok(user.PictureUrl, "Profile picture retrieved successfully");
    }
    public async Task<Result<object>> DeleteProfilePictureAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return Result<object>.BadRequest("User not found");

        if (string.IsNullOrEmpty(user.PictureUrl))
            return Result<object>.BadRequest("No profile picture to delete");

        var deletionResult = await _cloudinaryServices.DeleteImageAsync(user.PictureUrl);
        if (!deletionResult)
            return Result<object>.BadRequest("Failed to delete profile picture from cloud storage");

        user.PictureUrl = null;
        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
            return Result<object>.BadRequest("Failed to update user profile", updateResult.Errors);

        return Result<object>.Ok("Profile picture deleted successfully");
    }
}
