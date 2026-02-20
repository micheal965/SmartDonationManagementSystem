namespace SmartDonationSystem.Core.Common.Models
{
    public class Category : BaseEntity
    {
        public required string Name { get; set; }
        public List<Post>? Posts { get; set; } = new List<Post>();
    }
}
