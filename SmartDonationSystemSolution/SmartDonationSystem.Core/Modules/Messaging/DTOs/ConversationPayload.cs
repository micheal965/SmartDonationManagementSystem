namespace SmartDonationSystem.Core.Modules.Messaging.DTOs
{
    public class ConversationPayload
    {
        public int Id { get; set; }

        public string OtherUserId { get; set; }
        public string OtherUserName { get; set; }
        public string? OtherUserImage { get; set; }

        public string? LastMessage { get; set; }
        public DateTime? LastMessageAt { get; set; }
    }
}
