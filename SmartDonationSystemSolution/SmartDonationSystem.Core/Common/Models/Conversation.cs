namespace SmartDonationSystem.Core.Common.Models
{
    public class Conversation : BaseEntity
    {
        public string User1Id { get; set; }
        public string User2Id { get; set; }

        public DateTime CreatedAt { get; set; }

        // UI optimization
        public string? LastMessage { get; set; }
        public DateTime? LastMessageAt { get; set; }
        public bool lastMessageIsRead { get; set; } = false;
        public int User1UnreadCount { get; set; }
        public int User2UnreadCount { get; set; }

        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
