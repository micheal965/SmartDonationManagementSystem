namespace SmartDonationSystem.Core.Common.Models
{
    public class Comment : BaseEntity
    {
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }

        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public int PostId { get; set; }
        public Post Post { get; set; }

        public int? ParentCommentId { get; set; }
        public Comment ParentComment { get; set; }

        public ICollection<Comment> Replies { get; set; }
        public ICollection<CommentTag> Mentions { get; set; }

    }
}
