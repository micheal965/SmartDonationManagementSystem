using SmartDonationSystem.Shared.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartDonationSystem.Core.Common.Models
{
    public class Post : BaseEntity
    {
        public required string Title { get; set; }
        public required string Content { get; set; }
        public string Status { get; set; } = PostStatus.Pending.ToString();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? FreezedAt { get; set; }
        public string? FreezedBy { get; set; }
        public bool IsFreezed { get; set; } = false;
        public required string PostPicture { get; set; }

        #region Ranking
        public double? ImpactScore { get; set; }
        public int? PriorityLevel { get; set; }
        public string? AiSummary { get; set; }
        public DateTime? LastScoredAt { get; set; }
        #endregion

        [ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public List<Reaction>? Reactions { get; set; } = new List<Reaction>();
        public List<Comment>? Comments { get; set; } = new List<Comment>();
        public List<PostAttachment>? PostAttachments { get; set; } = new List<PostAttachment>();
        public List<AnalyticsEvent>? AnalyticsEvents { get; set; } = new List<AnalyticsEvent>();
    }
}
