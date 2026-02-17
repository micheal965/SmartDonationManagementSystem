using SmartDonationSystem.Core.Common;

namespace SmartDonationSystem.Core.Modules.Post.Models
{
    public class Post : BaseEntity
    {
        public required string Title { get; set; }
        public required string Content { get; set; }
        public string Status { get; set; } = "pending";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? FreezedAt { get; set; }
        public string? FreezedBy { get; set; }
        public bool IsFreezed { get; set; } = false;
    }
}
