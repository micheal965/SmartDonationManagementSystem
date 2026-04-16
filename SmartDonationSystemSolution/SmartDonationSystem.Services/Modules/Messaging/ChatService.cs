using Microsoft.EntityFrameworkCore;
using SmartDonationSystem.Core.Common.Models;
using SmartDonationSystem.Core.Modules.Encryption.Interfaces;
using SmartDonationSystem.Core.Modules.Messaging.DTOs;
using SmartDonationSystem.Core.Modules.Messaging.Interfaces;
using SmartDonationSystem.DataAccess;
using SmartDonationSystem.Shared.EncryptionPurposes;
using SmartDonationSystem.Shared.Pagination;
using SmartDonationSystem.Shared.Responses;

namespace SmartDonationSystem.Services.Modules.Messaging
{
    public class ChatService(ApplicationDbContext _context, IEncryptionService _encryptionService) : IChatService
    {
        public async Task<Result<List<ConversationPayload>>> GetUserConversationsAsync(string userId)
        {
            var conversations = await _context.Conversations
                .Where(c => c.User1Id == userId || c.User2Id == userId)
                .OrderByDescending(c => c.LastMessageAt)
                .ToListAsync();

            var otherUserIds = conversations
                .Select(c => c.User1Id == userId ? c.User2Id : c.User1Id)
                .Distinct()
                .ToList();

            var users = await _context.Users
                .Where(u => otherUserIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id);

            var result = conversations.Select(c =>
            {
                var otherUserId = c.User1Id == userId ? c.User2Id : c.User1Id;

                users.TryGetValue(otherUserId, out var otherUser);

                return new ConversationPayload
                {
                    Id = c.Id,
                    OtherUserId = otherUserId,
                    OtherUserName = otherUser?.FullName ?? "Unknown",
                    OtherUserImage = otherUser?.PictureUrl,
                    LastMessage = c.LastMessage != null ? _encryptionService.Decrypt(c.LastMessage, EncryptionPurposes.ChatMessages) : null,
                    LastMessageAt = c.LastMessageAt,
                    lastMessageIsRead = c.lastMessageIsRead
                };

            }).ToList();

            return Result<List<ConversationPayload>>.Ok(result);
        }
        public async Task<Result<ConversationPayload>> GetOrCreateConversationAsync(string userId, string receiverId)
        {
            var receiver = await _context.Users.FindAsync(receiverId);
            if (receiver == null)
                return Result<ConversationPayload>.NotFound("Receiver not found");

            var conversation = await GetOrCreateConversation(userId, receiverId);

            var result = new ConversationPayload
            {
                Id = conversation.Id,
                OtherUserId = receiver.Id,
                OtherUserName = receiver?.FullName ?? "Unknown",
                OtherUserImage = receiver?.PictureUrl,
                LastMessage = conversation.LastMessage != null ? _encryptionService.Decrypt(conversation.LastMessage, EncryptionPurposes.ChatMessages) : null,
                LastMessageAt = conversation.LastMessageAt
            };

            return Result<ConversationPayload>.Ok(result);
        }
        public async Task<MessagePayload> SendMessageAsync(SendMessageRequest request)
        {

            var conversation = await GetOrCreateConversation(request.SenderId, request.ReceiverId);

            var encryptedContent = _encryptionService.Encrypt(
                request.Content,
                EncryptionPurposes.ChatMessages
            );

            conversation.LastMessage = encryptedContent;
            conversation.LastMessageAt = DateTime.UtcNow;

            var message = new Message
            {
                ConversationId = conversation.Id,
                SenderId = request.SenderId,
                Content = encryptedContent,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Messages.AddAsync(message);

            await _context.SaveChangesAsync();

            return new MessagePayload
            {
                Id = message.Id,
                SenderId = request.SenderId,
                ReceiverId = request.ReceiverId,
                ConversationId = conversation.Id,
                Content = request.Content,
                CreatedAt = message.CreatedAt,
                IsRead = message.IsRead,
            };
        }
        public async Task<Result<PaginatedList<MessagePayload>>> GetMessagesAsync(
            string userId,
            int conversationId,
            int page = 1,
            int pageSize = 20)
        {
            var query = _context.Messages
                .Where(m => m.ConversationId == conversationId);

            var totalCount = await query.CountAsync();

            var messages = await query
                .OrderByDescending(m => m.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            messages.Reverse();

            var payload = messages.Select(m => new MessagePayload
            {
                Id = m.Id,
                ConversationId = m.ConversationId,
                Content = _encryptionService.Decrypt(m.Content, EncryptionPurposes.ChatMessages),
                CreatedAt = m.CreatedAt,
                IsMine = m.SenderId == userId,
                IsRead = m.IsRead

            }).ToList();

            return Result<PaginatedList<MessagePayload>>.Ok(
                new PaginatedList<MessagePayload>(payload, page, pageSize, totalCount)
            );
        }

        public async Task<Conversation> GetOrCreateConversation(string userA, string userB)
        {

            var (u1, u2) = userA.CompareTo(userB) < 0
                ? (userA, userB)
                : (userB, userA);

            var conversation = await _context.Conversations
                .FirstOrDefaultAsync(c => c.User1Id == u1 && c.User2Id == u2);

            if (conversation != null)
                return conversation;

            conversation = new Conversation
            {
                User1Id = u1,
                User2Id = u2,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Conversations.AddAsync(conversation);
            await _context.SaveChangesAsync();

            return conversation;
        }
        public async Task MarkConversationAsRead(int conversationId, string userId)
        {
            await _context.Messages
                .Where(m =>
                    m.ConversationId == conversationId &&
                    m.SenderId != userId &&
                    !m.IsRead)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(m => m.IsRead, true)
                    .SetProperty(m => m.ReadAt, DateTime.UtcNow)
                );

            await _context.Conversations
                .Where(c => c.Id == conversationId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(c => c.lastMessageIsRead, true)
                );
        }
        public async Task<string> GetOtherParticipant(int conversationId, string currentUserId)
        {
            var conversation = await _context.Conversations
                .AsNoTracking()
                .FirstOrDefaultAsync(c =>
                    c.Id == conversationId &&
                    (c.User1Id == currentUserId || c.User2Id == currentUserId));

            if (conversation == null)
                throw new Exception("Conversation not found or access denied");

            return conversation.User1Id == currentUserId
                ? conversation.User2Id
                : conversation.User1Id;
        }

    }
}
