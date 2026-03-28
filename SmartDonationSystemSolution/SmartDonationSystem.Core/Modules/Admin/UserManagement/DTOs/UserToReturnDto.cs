namespace SmartDonationSystem.Core.Modules.Admin.UserManagement.DTOs
{
    public class UserToReturnDto
    {
        public required string Id { get; set; }
        public required string FullName { get; set; }
        public required string IdentityNumber { get; set; }
        public string? PictureUrl { get; set; }
        public DateOnly BirthDate { get; set; }
        public string? Address { get; set; }
        public bool IsSoftDeleted { get; set; }
        public required string role { get; set; }
    }
}
