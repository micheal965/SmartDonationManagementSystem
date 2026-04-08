using SmartDonationSystem.Shared.Enums;

namespace SmartDonationSystem.Core.Modules.Notifications.DTOs
{
    public class CreateNotificationRequest
    {
        public string ReceiverId { get; set; }
        public string? ActorId { get; set; }

        public string Title { get; set; }
        public string Message { get; set; }

        public NotificationType Type { get; set; }

        public int EntityId { get; set; }

        public string? RedirectUrl { get; set; }

        public string? ActorName { get; set; }
        public string? ActorImage { get; set; }
    }
}
