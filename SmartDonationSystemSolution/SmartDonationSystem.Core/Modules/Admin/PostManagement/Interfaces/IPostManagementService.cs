using SmartDonationSystem.Core.Modules.Admin.PostManagement.DTOs;
using SmartDonationSystem.Shared.Responses;

namespace SmartDonationSystem.Core.Modules.Admin.PostManagement.Interfaces
{
    public interface IPostManagementService
    {
        Task<Result<List<PostToReturnDto>>> GetPendingAndFreezedPostsAsync();
        Task<Result<object>> ApproveOrFreezePostAsync(int postId, string action, string applicationUserId);
    }
}
