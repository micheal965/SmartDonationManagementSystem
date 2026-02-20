using System.ComponentModel.DataAnnotations.Schema;

namespace SmartDonationSystem.Core.Common.Models
{
    public class Reaction : BaseEntity
    {
        public DateTime CreatedAt { get; set; }

        [ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        [ForeignKey("Post")]
        public int PostId { get; set; }
        public Post Post { get; set; }
    }
}
