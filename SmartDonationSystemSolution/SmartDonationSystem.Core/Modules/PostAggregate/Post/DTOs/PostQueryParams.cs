using SmartDonationSystem.Shared.Enums;

namespace SmartDonationSystem.Core.Modules.PostAggregate.Post.DTOs
{
    public class PostQueryParams
    {
        public int pageNumber { get; set; } = 1;
        public int pageSize { get; set; } = 5;
        public string? categoryName { get; set; }
        public PostSortBy sortBy { get; set; } = PostSortBy.Urgent;
    }
}
