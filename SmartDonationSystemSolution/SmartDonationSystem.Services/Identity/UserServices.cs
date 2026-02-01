using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SmartDonationSystem.Core.Auth.Models;
using SmartDonationSystem.Core.Cloud;
using SmartDonationSystem.Core.User.DTOs;
using SmartDonationSystem.Core.User.Interfaces;
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
        if (user == null) return Result<object>.NotFound("User not found");

        var passwordChangeResult = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
        if (!passwordChangeResult.Succeeded)
            return Result<object>.BadRequest("Cannot Change Password!", passwordChangeResult.Errors.Select(e => e.Description));

        //forces re-login on other devices.
        await _userManager.UpdateSecurityStampAsync(user);
        return Result<object>.Ok("Password changed successfully");
    }
    public async Task<Result<object>> AddOrUpdateProfilePictureAsync(string userId, IFormFile profilePicture)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return Result<object>.NotFound("User not found");

        var uploadResult = await _cloudinaryServices.UploadImageAsync(profilePicture);
        if (!uploadResult.isSucceded)
            return Result<object>.BadRequest("Failed to upload profile picture");

        if (user.PictureUrl != null)
            await _cloudinaryServices.DeleteImageAsync(user.PictureUrl);

        user.PictureUrl = uploadResult.url;
        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
            return Result<object>.BadRequest("Failed to update user profile", updateResult.Errors);

        return Result<object>.Ok("Profile picture updated successfully");
    }
    public async Task<Result<string>> GetProfilePictureAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return Result<string>.NotFound("User not found");

        if (string.IsNullOrEmpty(user.PictureUrl))
            return Result<string>.NotFound("No profile picture set");

        return Result<string>.Ok(user.PictureUrl, "Profile picture retrieved successfully");
    }
    public async Task<Result<object>> DeleteProfilePictureAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return Result<object>.NotFound("User not found");

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
    public async Task<Result<object>> DeleteUserSoftAsync(string userId)
    {
        ApplicationUser? user = await _userManager.FindByIdAsync(userId);
        if (user == null) return Result<object>.NotFound("User not found");

        user.IsSoftDeleted = true;

        var deleteResult = await _userManager.UpdateAsync(user);
        if (!deleteResult.Succeeded)
            return Result<object>.BadRequest("Failed to delete user", deleteResult.Errors);

        //forces Logout on other devices.
        await _userManager.UpdateSecurityStampAsync(user);
        return Result<object>.Ok("User deleted successfully");
    }
    public async Task<Result<UserToReturnDto>> GetSpecificUserAsync(string userId)
    {
        ApplicationUser? user = await _userManager.FindByIdAsync(userId);
        if (user == null) return Result<UserToReturnDto>.NotFound("User not found");

        return Result<UserToReturnDto>.Ok(user.Adapt<UserToReturnDto>(), "User retrieved successfully");
    }
    public async Task<Result<object>> UpdateUserAsync(string? userId, UpdateUserRequestDto updateUserRequestDto)
    {
        ApplicationUser? user = await _userManager.FindByIdAsync(userId);
        if (user == null) return Result<object>.NotFound("User not found");

        user.FullName = updateUserRequestDto.FullName ?? user.FullName;
        user.PhoneNumber = updateUserRequestDto.PhoneNumber ?? user.PhoneNumber;

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
            return Result<object>.BadRequest("Failed to update user", updateResult.Errors);

        return Result<object>.Ok("User updated successfully");
    }
}
