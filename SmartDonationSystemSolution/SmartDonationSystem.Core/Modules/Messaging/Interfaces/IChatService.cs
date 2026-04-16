using SmartDonationSystem.Core.Modules.Messaging.DTOs;
using SmartDonationSystem.Shared.Pagination;
using SmartDonationSystem.Shared.Responses;

namespace SmartDonationSystem.Core.Modules.Messaging.Interfaces
{
    public interface IChatService
    {
        Task<MessagePayload> SendMessageAsync(SendMessageRequest request);
        Task<Result<PaginatedList<MessagePayload>>> GetMessagesAsync(string userId, int conversationId, int page = 1, int pageSize = 20);
        Task<Result<List<ConversationPayload>>> GetUserConversationsAsync(string userId);
        Task<Result<ConversationPayload>> GetOrCreateConversationAsync(string userAId, string userBId);
        Task MarkConversationAsRead(int conversationId, string userId);
        Task<string> GetOtherParticipant(int conversationId, string currentUserId);
    }
}
