namespace SmartDonationSystem.Core.Modules.Admin.UserManagement.DTOs
{
    public class UpdateUserDto
    {
        public string Id { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string IdentityNumber { get; set; } = string.Empty;
        public string PictureUrl { get; set; } = string.Empty;

        public DateOnly BirthDate { get; set; }

        public string Address { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
    }
}
