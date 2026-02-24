namespace SmartDonationSystem.Core.Modules.User.PostAggregate.Post.DTOs
{
    public class PostToReturnDto
    {
        public required int id { get; set; }
        public required string title { get; set; }
        public required string content { get; set; }
        public DateTime createdAt { get; set; }
        public int? priorityLevel { get; set; }
        public int? likesCount { get; set; }
        public List<string> attachments { get; set; }
        public bool hasReacted { get; set; }

        public required string userId { get; set; }
        public required string fullName { get; set; }
        public required string pictureUrl { get; set; }
    }
}
