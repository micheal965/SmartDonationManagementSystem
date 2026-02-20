using System.ComponentModel.DataAnnotations.Schema;

namespace SmartDonationSystem.Core.Common.Models
{
    public class PostAttachment
    {
        public required string AttachmentUrl { get; set; }

        [ForeignKey("Post")]
        public int PostId { get; set; }
        public Post Post { get; set; }
    }
}
