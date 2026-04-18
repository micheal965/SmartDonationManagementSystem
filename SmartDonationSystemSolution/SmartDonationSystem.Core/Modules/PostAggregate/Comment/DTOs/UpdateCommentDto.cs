using System.ComponentModel.DataAnnotations;

namespace SmartDonationSystem.Core.Modules.PostAggregate.Comment.DTOs
{
    public class UpdateCommentDto
    {
        [Required(ErrorMessage = "Comment content is required.")]
        [MaxLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters.")]
        public string Content { get; set; }
        public List<string>? MentionedUserIds { get; set; }

    }
}
