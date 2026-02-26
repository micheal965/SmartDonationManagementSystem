using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SmartDonationSystem.Core.Modules.User.PostAggregate.Post.DTOs
{
    public class CreatePostDto
    {
        [Required]
        public string title { get; set; }
        [Required]
        public string content { get; set; }
        [Required]
        public int categoryId { get; set; }
        public List<IFormFile>? attachments { get; set; }
    }
}
