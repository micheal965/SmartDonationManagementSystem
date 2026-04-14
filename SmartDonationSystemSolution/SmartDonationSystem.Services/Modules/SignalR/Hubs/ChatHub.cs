using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SmartDonationSystem.Core.Modules.Messaging.DTOs;
using SmartDonationSystem.Core.Modules.Messaging.Interfaces;

namespace SmartDonationSystem.Services.Modules.SignalR.Hubs
{
    [Authorize]
    public class ChatHub(IChatService _chatService) : Hub
    {
        public async Task SendMessage(SendMessageRequest request)
        {
            request.SenderId = Context.UserIdentifier;
            var message = await _chatService.SendMessageAsync(request);
            await Clients.Users(new[] { request.SenderId, request.ReceiverId }).SendAsync("ReceiveMessage", message);
        }

        public async Task JoinConversation(int conversationId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId.ToString());
        }
    }
}
