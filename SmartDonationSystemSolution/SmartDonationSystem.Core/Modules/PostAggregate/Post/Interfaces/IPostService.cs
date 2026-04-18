using SmartDonationSystem.Core.Modules.PostAggregate.Post.DTOs;
using SmartDonationSystem.Shared.Enums;
using SmartDonationSystem.Shared.Pagination;
using SmartDonationSystem.Shared.Responses;

namespace SmartDonationSystem.Core.Modules.PostAggregate.Post.Interfaces
{
    public interface IPostService
    {
        Task<Result<object>> CreatePostAsync(CreatePostDto createPostDto, string applicationUserId);
        Task<Result<PaginatedList<PostToReturnDto>>> GetPostsAsync(string userId, int pageNumber, int pageSize, string? categoryName, PostSortBy sortBy);
        Task<Result<PostToReturnDto>> GetPostAsync(int postId);
        Task<Result<object>> TrackPostViewAsync(string userId, int postId);
    }
}
