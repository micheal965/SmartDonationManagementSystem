namespace SmartDonationSystem.Core.Modules.Admin.PostManagement.DTOs
{
    public class PostToReturnDto
    {
        public required string Id { get; set; }
        public required string Title { get; set; }
        public required string Content { get; set; }
        public required string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? FreezedBy { get; set; }
        public required string CategoryName { get; set; }
        public List<string>? PostAttachments { get; set; }

    }
}
