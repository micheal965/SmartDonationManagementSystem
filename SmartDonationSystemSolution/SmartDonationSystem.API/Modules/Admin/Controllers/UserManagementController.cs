using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartDonationSystem.Core.Modules.Admin.UserManagement.DTOs;
using SmartDonationSystem.Core.Modules.Admin.UserManagement.Interfaces;
using SmartDonationSystem.Shared.Enums;

namespace SmartDonationSystem.API.Modules.Admin.Controllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = AppRoles.Admin)]

    public class UserManagementController(IUserManagementService _userManagementService) : ControllerBase
    {
        [HttpGet("get-users")]
        public async Task<IActionResult> GetUsers([FromQuery] string? role, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            var result = await _userManagementService.GetUsersAsync(pageNumber, pageSize, role);
            return StatusCode((int)result.statusCode, result);
        }
        [HttpDelete("toggle-user-soft-delete")]
        public async Task<IActionResult> ToggleUserSoftDelete([FromQuery] string userId)
        {
            var deleteUserSoftResponse = await _userManagementService.ToggleUserSoftDeleteAsync(userId);
            return StatusCode((int)deleteUserSoftResponse.statusCode, deleteUserSoftResponse);
        }
        [HttpPost("add-new-user")]
        public async Task<IActionResult> AddNewUser(RegisterUserDto dto)
        {
            var result = await _userManagementService.AddNewUserAsync(dto);
            return StatusCode((int)result.statusCode, result);
        }
        [HttpPut("update-user")]
        public async Task<IActionResult> UpdateUser(UpdateUserDto dto)
        {
            var result = await _userManagementService.UpdateUserAsync(dto);
            return StatusCode((int)result.statusCode, result);
        }
        [HttpGet("get-user-details")]
        public async Task<IActionResult> GetUsers([FromQuery] string id)
        {
            var result = await _userManagementService.GetUserByIdAsync(id);
            return StatusCode((int)result.statusCode, result);
        }
    }
}
