using System.ComponentModel.DataAnnotations;

namespace SmartDonationSystem.Core.Modules.User.PostAggregate.Comment.DTOs
{
    public class CreateCommentDto
    {
        [Required]
        public string Content { get; set; }
        [Required]
        public int PostId { get; set; }
        public int? ParentCommentId { get; set; }
    }
}
