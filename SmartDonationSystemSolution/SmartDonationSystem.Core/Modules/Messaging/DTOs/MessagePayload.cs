namespace SmartDonationSystem.Core.Modules.Messaging.DTOs
{
    public class MessagePayload
    {
        public int Id { get; set; }
        public string SenderId { get; set; } = default!;
        public string ReceiverId { get; set; } = default!;
        public int ConversationId { get; set; }

        public string Content { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public bool IsMine { get; set; }
        public bool IsRead { get; set; }
        public MessageParticipantsPayload? Participants { get; set; }
    }
}
