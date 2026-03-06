namespace SmartDonationSystem.Core.Common.Models
{
    public class CommentTag : BaseEntity
    {
        public int CommentId { get; set; }
        public Comment Comment { get; set; }

        public string MentionedUserId { get; set; }
        public ApplicationUser MentionedUser { get; set; }
    }
}
