using SmartDonationSystem.Shared.Enums;

namespace SmartDonationSystem.Core.Common.Models
{
    public class Notification : BaseEntity
    {
        public required string ReceiverId { get; set; }
        public string? ActorId { get; set; }

        public string Title { get; set; }
        public string Message { get; set; }

        public NotificationType Type { get; set; } // Like, Comment, Post, etc.

        public bool IsRead { get; set; } = false;
        public bool IsAllMarkedAsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int EntityId { get; set; } // PostId, CommentId, PostId, etc.

        //Snapshot
        public string? ActorName { get; set; }
        public string? ActorImage { get; set; }
        public string? RedirectUrl { get; set; }
    }
}