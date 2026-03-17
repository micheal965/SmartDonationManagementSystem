namespace SmartDonationSystem.Core.Modules.User.Profile.DTOs
{
    public class UserCommentDto
    {
        public required int PostId { get; set; }
        public required string Content { get; set; }
        public required DateTime CreatedAt { get; set; }
    }
}
