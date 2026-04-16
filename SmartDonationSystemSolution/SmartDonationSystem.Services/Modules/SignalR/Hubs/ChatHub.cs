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
            await Clients.Users(new[] { request.SenderId!, request.ReceiverId })
                .SendAsync("ReceiveMessage", message);
        }
        public async Task Typing(TypingRequest request)
        {
            var senderId = Context.UserIdentifier;

            await Clients.User(request.ReceiverId)
                .SendAsync("UserTyping", new
                {
                    SenderId = senderId
                });
        }
        public async Task MarkAsRead(int conversationId)
        {
            var userId = Context.UserIdentifier;
            var receiverId = await _chatService.GetOtherParticipant(conversationId, userId);

            await _chatService.MarkConversationAsRead(conversationId, userId);

            await Clients.User(receiverId).SendAsync("MessagesRead", new
            {
                conversationId,
                userId
            });
        }
    }
}
