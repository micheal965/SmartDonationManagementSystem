using SmartDonationSystem.Shared.Enums;

namespace SmartDonationSystem.Core.Common.Models
{
    public sealed class AnalyticsEvent : BaseEntity
    {
        public AnalyticsEventType Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? PostId { get; set; }
        public Post? Post { get; set; }
        public string? ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
    }
}
