using Microsoft.AspNetCore.Http;

namespace SmartDonationSystem.Core.Modules.User.PostAggregate.Post.DTOs
{
    public class CreatePostDto
    {
        public required string title { get; set; }
        public required string content { get; set; }
        public required int categoryId { get; set; }
        public required List<IFormFile> attachments { get; set; }
    }
}
