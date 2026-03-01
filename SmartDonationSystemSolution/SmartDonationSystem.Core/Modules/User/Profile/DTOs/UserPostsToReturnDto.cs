namespace SmartDonationSystem.Core.Modules.User.Profile.DTOs
{
    public class UserPostsToReturnDto
    {
        public required int totalPostsCount { get; set; }
        public List<UserPostDto>? posts { get; set; }
    }

}
