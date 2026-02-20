namespace SmartDonationSystem.Core.Modules.User.PostAggregate.Post.DTOs
{
    public class PostToReturnDto
    {
        public required int Id { get; set; }
        public required string Title { get; set; }
        public required string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? PriorityLevel { get; set; }
    }
}
