using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartDonationSystem.Core.User.DTOs;
using SmartDonationSystem.Core.User.Interfaces;
using System.Security.Claims;

namespace SmartDonationSystem.API.Identity.User.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userServices;

        public UserController(IUserServices userServices)
        {
            _userServices = userServices;
        }
        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequestDto changePasswordRequestDto)
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var changePasswordResponse = await _userServices.ChangePasswordAsync(userId, changePasswordRequestDto.OldPassword, changePasswordRequestDto.NewPassword);
            return StatusCode((int)changePasswordResponse.statusCode, changePasswordResponse);
        }
        [HttpPost("set-profile-picture")]
        [Authorize]
        public async Task<IActionResult> SetProfilePicture(IFormFile profilePicture)
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var setProfilePictureResponse = await _userServices.AddOrUpdateProfilePictureAsync(userId, profilePicture);
            return StatusCode((int)setProfilePictureResponse.statusCode, setProfilePictureResponse);
        }
        [HttpGet("get-profile-picture")]
        public async Task<IActionResult> GetProfilePicture([FromQuery] string userId)
        {
            var getProfilePictureResponse = await _userServices.GetProfilePictureAsync(userId);
            return StatusCode((int)getProfilePictureResponse.statusCode, getProfilePictureResponse);
        }
        [HttpDelete("delete-profile-picture")]
        [Authorize]
        public async Task<IActionResult> DeleteProfilePicture()
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var deleteProfilePictureResponse = await _userServices.DeleteProfilePictureAsync(userId);
            return StatusCode((int)deleteProfilePictureResponse.statusCode, deleteProfilePictureResponse);
        }
        [HttpDelete("delete-user-soft")]
        [Authorize]
        public async Task<IActionResult> DeleteUserSoft()
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var deleteUserSoftResponse = await _userServices.DeleteUserSoftAsync(userId);
            return StatusCode((int)deleteUserSoftResponse.statusCode, deleteUserSoftResponse);
        }
        [HttpGet("get-user-data")]
        public async Task<IActionResult> GetUserData([FromQuery] string userId)
        {
            var getUserDataResponse = await _userServices.GetSpecificUserAsync(userId);
            return StatusCode((int)getUserDataResponse.statusCode, getUserDataResponse);
        }
        [HttpPut("update-user")]
        [Authorize]
        public async Task<IActionResult> UpdateUserData(UpdateUserRequestDto updateUserRequestDto)
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var getUserDataResponse = await _userServices.UpdateUserAsync(userId, updateUserRequestDto);
            return StatusCode((int)getUserDataResponse.statusCode, getUserDataResponse);
        }

    }
}
