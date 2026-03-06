namespace SmartDonationSystem.Core.Modules.User.PostAggregate.Comment.DTOs
{
    public class CommentDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string UserName { get; set; }
        public string creatorPictureUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<CommentDto> Replies { get; set; }
    }
}
