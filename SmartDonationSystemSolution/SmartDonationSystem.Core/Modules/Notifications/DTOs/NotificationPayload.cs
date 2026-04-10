namespace SmartDonationSystem.Core.Modules.Notifications.DTOs
{
    public class NotificationPayload
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public int? EntityId { get; set; }
        public string? RedirectUrl { get; set; }
        public string Type { get; set; }
        public string? ActorName { get; set; }
        public string? ActorImage { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
    }
}
