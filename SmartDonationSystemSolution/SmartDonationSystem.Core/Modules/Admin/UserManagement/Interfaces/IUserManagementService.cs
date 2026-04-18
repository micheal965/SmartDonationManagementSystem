using SmartDonationSystem.Core.Modules.Admin.UserManagement.DTOs;
using SmartDonationSystem.Shared.Pagination;
using SmartDonationSystem.Shared.Responses;

namespace SmartDonationSystem.Core.Modules.Admin.UserManagement.Interfaces
{
    public interface IUserManagementService
    {
        Task<Result<PaginatedList<UserToReturnDto>>> GetUsersAsync(int pageNumber, int pageSize, string? role);
        Task<Result<object>> ToggleUserSoftDeleteAsync(string userId);
        Task<Result<object>> AddNewUserAsync(RegisterUserDto dto);
        Task<Result<UserToReturnDto>> UpdateUserAsync(UpdateUserDto dto);
        Task<Result<UserToReturnDto>> GetUserByIdAsync(string id);
    }
}
