using SmartDonationSystem.Core.Modules.Admin.UserManagement.DTOs;
using SmartDonationSystem.Shared.Pagination;
using SmartDonationSystem.Shared.Responses;

namespace SmartDonationSystem.Core.Modules.Admin.UserManagement.Interfaces
{
    public interface IUserManagementService
    {
        Task<Result<PaginatedList<UserToReturnDto>>> GetUsersAsync(int pageNumber, int pageSize, string? role);
        Task<Result<object>> ToggleUserSoftDeleteAsync(string userId);

    }
}
