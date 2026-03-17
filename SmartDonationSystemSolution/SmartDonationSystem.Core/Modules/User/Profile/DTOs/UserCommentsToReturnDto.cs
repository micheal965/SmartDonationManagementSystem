namespace SmartDonationSystem.Core.Modules.User.Profile.DTOs
{
    public class UserCommentsToReturnDto
    {
        public required int totalCommentsCount { get; set; }
        public List<UserCommentDto>? comments { get; set; }
    }
}
