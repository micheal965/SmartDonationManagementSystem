using SmartDonationSystem.Core.Modules.Admin.PostManagement.DTOs;
using SmartDonationSystem.Shared.Enums;
using SmartDonationSystem.Shared.Pagination;
using SmartDonationSystem.Shared.Responses;

namespace SmartDonationSystem.Core.Modules.Admin.PostManagement.Interfaces
{
    public interface IPostManagementService
    {
        Task<Result<PaginatedList<PostToReturnDto>>> GetPostsAsync(int pageNumber, int pageSize, PostStatus? postStatus);
        Task<Result<PostToReturnDto>> GetPostByIdAsync(int postId);
        Task<Result<object>> ApproveOrFreezePostAsync(int postId, string action, string applicationUserId);
    }
}
