using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartDonationSystem.Core.Modules.Notifications.Interfaces;
using System.Security.Claims;

namespace SmartDonationSystem.API.Modules.Notifications.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("get-user-notifications")]
        public async Task<IActionResult> GetUserNotifications([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _notificationService.GetUserNotificationsAsync(userId, page, pageSize);

            return StatusCode((int)result.statusCode, result);
        }
        [HttpPut("read")]
        public async Task<IActionResult> MarkAsRead([FromQuery] int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _notificationService.MarkAsReadAsync(userId, id);
            return Ok();
        }
        [HttpPut("mark-all-read")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _notificationService.MarkAllAsReadAsync(userId);
            return Ok();
        }

    }
}
