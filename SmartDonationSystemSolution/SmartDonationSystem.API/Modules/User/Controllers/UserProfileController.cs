using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartDonationSystem.Core.Modules.User.Profile.DTOs;
using SmartDonationSystem.Core.Modules.User.Profile.Interfaces;
using System.Security.Claims;

namespace SmartDonationSystem.API.Modules.User.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService _userProfileServices;

        public UserProfileController(IUserProfileService userServices)
        {
            _userProfileServices = userServices;
        }
        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequestDto changePasswordRequestDto)
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var changePasswordResponse = await _userProfileServices.ChangePasswordAsync(userId, changePasswordRequestDto.OldPassword
                                                                                            , changePasswordRequestDto.NewPassword);
            return StatusCode((int)changePasswordResponse.statusCode, changePasswordResponse);
        }
        [HttpPost("set-profile-picture")]
        [Authorize]
        public async Task<IActionResult> SetProfilePicture(IFormFile profilePicture)
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var setProfilePictureResponse = await _userProfileServices.AddOrUpdateProfilePictureAsync(userId, profilePicture);
            return StatusCode((int)setProfilePictureResponse.statusCode, setProfilePictureResponse);
        }
        [HttpGet("get-profile-picture")]
        public async Task<IActionResult> GetProfilePicture([FromQuery] string userId)
        {
            var getProfilePictureResponse = await _userProfileServices.GetProfilePictureAsync(userId);
            return StatusCode((int)getProfilePictureResponse.statusCode, getProfilePictureResponse);
        }
        [HttpDelete("delete-profile-picture")]
        [Authorize]
        public async Task<IActionResult> DeleteProfilePicture()
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var deleteProfilePictureResponse = await _userProfileServices.DeleteProfilePictureAsync(userId);
            return StatusCode((int)deleteProfilePictureResponse.statusCode, deleteProfilePictureResponse);
        }
        [HttpDelete("delete-user-soft")]
        [Authorize]
        public async Task<IActionResult> DeleteUserSoft()
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var deleteUserSoftResponse = await _userProfileServices.DeleteUserSoftAsync(userId);
            return StatusCode((int)deleteUserSoftResponse.statusCode, deleteUserSoftResponse);
        }
        [HttpGet("get-user-data")]
        public async Task<IActionResult> GetUserData([FromQuery] string userId)
        {
            var getUserDataResponse = await _userProfileServices.GetSpecificUserAsync(userId);
            return StatusCode((int)getUserDataResponse.statusCode, getUserDataResponse);
        }
        [HttpPut("update-user")]
        [Authorize]
        public async Task<IActionResult> UpdateUserData(UpdateUserRequestDto updateUserRequestDto)
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var getUserDataResponse = await _userProfileServices.UpdateUserAsync(userId, updateUserRequestDto);
            return StatusCode((int)getUserDataResponse.statusCode, getUserDataResponse);
        }
        [Authorize]
        [HttpGet("login-history")]
        public async Task<IActionResult> GetLoginHistory()
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var historyListResult = await _userProfileServices.GetLoginHistoryAsync(userId);
            return StatusCode((int)historyListResult.statusCode, historyListResult);
        }
    }
}
