namespace SmartDonationSystem.Core.Modules.PostAggregate.Post.DTOs
{
    public class RecentDonationDto
    {
        public required string donorName { get; set; }
        public required string donorPictureUrl { get; set; }
        public decimal amount { get; set; }
        public DateTime createdAt { get; set; }
    }
}
