namespace SmartDonationSystem.Core.Common.Models
{
    public class Message : BaseEntity
    {
        public int ConversationId { get; set; }
        public Conversation Conversation { get; set; } = default!;

        public string SenderId { get; set; }

        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? ReadAt { get; set; }
        public bool IsRead { get; set; }
    }
}
