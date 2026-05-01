namespace SmartDonationSystem.Core.Modules.Admin.PostManagement.DTOs
{
    public class PostToReturnDto
    {
        public required int Id { get; set; }
        public required string Title { get; set; }
        public required string Content { get; set; }
        public required string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string PostPicture { get; set; }
        public required string CategoryName { get; set; }
        public List<string>? PostAttachments { get; set; }
        public required string CreatorName { get; set; }
        public required string creatorPicture { get; set; }
        public required string creatorRole { get; set; }
        public decimal? TargetMoney { get; set; }
    }
}
