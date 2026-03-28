using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartDonationSystem.Core.Common.Models;
using SmartDonationSystem.Core.Modules.Admin.UserManagement.DTOs;
using SmartDonationSystem.Core.Modules.Admin.UserManagement.Interfaces;
using SmartDonationSystem.DataAccess;
using SmartDonationSystem.Shared.Pagination;
using SmartDonationSystem.Shared.Responses;

namespace SmartDonationSystem.Services.Modules.Admin
{
    public class UserManagementService(ApplicationDbContext _context, UserManager<ApplicationUser> _userManager) : IUserManagementService
    {
        public async Task<Result<PaginatedList<UserToReturnDto>>> GetUsersAsync(
            int pageNumber, int pageSize, string? role)
        {
            var query = from u in _context.Users
                        join ur in _context.UserRoles on u.Id equals ur.UserId
                        join r in _context.Roles on ur.RoleId equals r.Id
                        select new UserToReturnDto
                        {
                            Id = u.Id,
                            Address = u.Address,
                            BirthDate = u.BirthDate,
                            FullName = u.FullName,
                            IdentityNumber = u.IdentityNumber,
                            IsSoftDeleted = u.IsSoftDeleted,
                            PictureUrl = u.PictureUrl,
                            role = r.Name
                        };

            if (!string.IsNullOrEmpty(role))
                query = query.Where(x => x.role == role);

            var totalCount = await query.CountAsync();

            var users = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new PaginatedList<UserToReturnDto>(users, pageNumber, pageSize, totalCount);

            return Result<PaginatedList<UserToReturnDto>>.Ok(result);
        }
        public async Task<Result<object>> ToggleUserSoftDeleteAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Result<object>.NotFound("User not found");

            user.IsSoftDeleted = user.IsSoftDeleted ? false : true;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return Result<object>.BadRequest("Failed to update user", updateResult.Errors);

            await _userManager.UpdateSecurityStampAsync(user);

            var message = user.IsSoftDeleted
                ? "User deactivated successfully"
                : "User restored successfully";

            return Result<object>.Ok(null, message);
        }
    }
}