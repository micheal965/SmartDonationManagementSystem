namespace SmartDonationSystem.Core.Modules.Messaging.DTOs
{
    public class MessagePayload
    {
        public int Id { get; set; }
        public int ConversationId { get; set; }
        public string SenderId { get; set; } = default!;
        public string? SenderImage { get; set; }
        public string? SenderName { get; set; }

        public string ReceiverId { get; set; } = default!;
        public string ReceiverName { get; set; }
        public string? ReceiverImage { get; set; }

        public string Content { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public bool IsMine { get; set; }
    }
}
