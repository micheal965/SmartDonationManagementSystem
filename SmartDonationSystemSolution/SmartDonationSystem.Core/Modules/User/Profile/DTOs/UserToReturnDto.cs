namespace SmartDonationSystem.Core.Modules.User.Profile.DTOs
{
    public class UserToReturnDto
    {
        public required string Id { get; set; }
        public required string FullName { get; set; }
        public required string PictureUrl { get; set; }
        public required DateOnly BirthDate { get; set; }
        public required string Address { get; set; }
        public required string PhoneNumber { get; set; }
        public required string role { get; set; }

    }
}
