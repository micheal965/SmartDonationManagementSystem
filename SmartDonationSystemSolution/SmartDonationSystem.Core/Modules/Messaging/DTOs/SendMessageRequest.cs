namespace SmartDonationSystem.Core.Modules.Messaging.DTOs
{
    public class SendMessageRequest
    {
        public int ConversationId { get; set; }
        public string SenderId { get; set; } = default!;
        public string ReceiverId { get; set; } = default!;
        public string Content { get; set; } = default!;
    }
}
