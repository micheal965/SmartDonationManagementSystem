namespace SmartDonationSystem.Core.Modules.User.Profile.DTOs
{
    public class UserPostDto
    {
        public required int id { get; set; }
        public required string title { get; set; }
        public required string content { get; set; }
        public required string postPicture { get; set; }
        public int? likesCount { get; set; }
    }
}
