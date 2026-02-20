namespace SmartDonationSystem.Core.Modules.User.Profile.DTOs
{
    public class UserToReturnDto
    {
        public required string Id { get; set; }
        public required string FullName { get; set; }
        public required string PictureUrl { get; set; }
        public required string BirthDate { get; set; }
    }
}
