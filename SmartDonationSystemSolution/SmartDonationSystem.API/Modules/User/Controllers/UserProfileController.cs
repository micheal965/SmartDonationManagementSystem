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

        [HttpGet("get-user-data")]
        public async Task<IActionResult> GetUserData([FromQuery] string userId)
        {
            var getUserDataResponse = await _userProfileServices.GetSpecificUserAsync(userId);
            return StatusCode((int)getUserDataResponse.statusCode, getUserDataResponse);
        }

        [HttpGet("search-user")]
        public async Task<IActionResult> Search([FromQuery] string query)
        {
            var result = await _userProfileServices.SearchUsersByNameAsync(query);
            return StatusCode((int)result.statusCode, result);
        }
        [HttpGet("get-user-posts")]
        public async Task<IActionResult> GetUserPosts([FromQuery] string userId)
        {
            var getUserPostsResponse = await _userProfileServices.GetUserPostsAsync(userId);
            return StatusCode((int)getUserPostsResponse.statusCode, getUserPostsResponse);
        }
        [HttpGet("get-user-comments")]
        public async Task<IActionResult> GetUserComments([FromQuery] string userId)
        {
            var getUserCommentsResponse = await _userProfileServices.GetUserCommentsAsync(userId);
            return StatusCode((int)getUserCommentsResponse.statusCode, getUserCommentsResponse);
        }
        [HttpGet("get-user-reactions")]
        public async Task<IActionResult> GetUserReactions([FromQuery] string userId)
        {
            var getUserReactionsResponse = await _userProfileServices.GetUserReactionsAsync(userId);
            return StatusCode((int)getUserReactionsResponse.statusCode, getUserReactionsResponse);
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
