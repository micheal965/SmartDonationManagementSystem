using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartDonationSystem.Core.Modules.Messaging.DTOs;
using SmartDonationSystem.Core.Modules.Messaging.Interfaces;
using System.Security.Claims;

namespace SmartDonationSystem.API.Modules.Messaging.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost("conversations/get-or-create")]
        public async Task<IActionResult> Get(GetOrCreateConversationRequest request)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var conversation = await _chatService.GetOrCreateConversation(userId, request.ReceiverId);
            return Ok(conversation.Id);
        }
        [HttpGet("conversations")]
        public async Task<IActionResult> GetConversations()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _chatService.GetUserConversationsAsync(userId);
            return StatusCode((int)result.statusCode, result);
        }

        [HttpGet("conversations/{conversationId}/messages")]
        public async Task<IActionResult> GetMessages(int conversationId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _chatService.GetMessagesAsync(userId, conversationId, page, pageSize);
            return StatusCode((int)result.statusCode, result);
        }

    }
}
