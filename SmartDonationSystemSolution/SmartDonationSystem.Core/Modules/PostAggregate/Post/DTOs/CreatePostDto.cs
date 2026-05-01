using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SmartDonationSystem.Core.Modules.PostAggregate.Post.DTOs
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
        [Required]
        public IFormFile PostPicture { get; set; }
        public decimal? targetMoney { get; set; }
    }
}
