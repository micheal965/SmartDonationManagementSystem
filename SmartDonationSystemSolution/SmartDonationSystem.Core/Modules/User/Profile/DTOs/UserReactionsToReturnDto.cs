namespace SmartDonationSystem.Core.Modules.User.Profile.DTOs
{
    public class UserReactionsToReturnDto
    {
        public required int totalLikesCount { get; set; }
        public List<UserReactionDto>? reactions { get; set; }
    }
}
