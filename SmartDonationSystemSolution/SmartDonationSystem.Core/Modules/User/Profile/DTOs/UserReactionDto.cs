namespace SmartDonationSystem.Core.Modules.User.Profile.DTOs
{
    public class UserReactionDto
    {
        public required int PostId { get; set; }
        public required string PostTitle { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
