using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartDonationSystem.Core.Common.Models;
using SmartDonationSystem.Core.Modules.Admin.UserManagement.DTOs;
using SmartDonationSystem.Core.Modules.Admin.UserManagement.Interfaces;
using SmartDonationSystem.DataAccess;
using SmartDonationSystem.Shared.Pagination;
using SmartDonationSystem.Shared.Responses;
using UserToReturnDto = SmartDonationSystem.Core.Modules.Admin.UserManagement.DTOs.UserToReturnDto;

namespace SmartDonationSystem.Services.Modules.Admin
{
    public class UserManagementService(ApplicationDbContext _context, UserManager<ApplicationUser> _userManager, RoleManager<IdentityRole> _roleManager)
        : IUserManagementService
    {

        public async Task<Result<object>> AddNewUserAsync(RegisterUserDto requestDto)
        {
            var identityNumber = requestDto.IdentityNumber.Trim();

            if (!await _roleManager.RoleExistsAsync(requestDto.Role))
                return Result<object>.BadRequest("Invalid role");

            bool existingUser = await _context.ApplicationUsers
                                .AnyAsync(u => u.IdentityNumber == identityNumber);
            if (existingUser) return Result<object>.BadRequest("A user with this Identity Number already exists.");

            ApplicationUser applicationUser = new ApplicationUser()
            {
                IdentityNumber = requestDto.IdentityNumber,
                FullName = requestDto.FullName,
                UserName = identityNumber,
                BirthDate = requestDto.BirthDate,
                PhoneNumber = requestDto.PhoneNumber,
                Address = requestDto.Address,
                PictureUrl = requestDto.ProfilePictureUrl,
            };


            IdentityResult createResult = await _userManager.CreateAsync(applicationUser, requestDto.Password);
            if (!createResult.Succeeded)
                return Result<object>.BadRequest("Registration failed", createResult.Errors.Select(e => e.Description));

            IdentityResult roleResult = await _userManager.AddToRoleAsync(applicationUser, requestDto.Role);
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(applicationUser);
                return Result<object>.BadRequest("Registration failed", roleResult.Errors.Select(e => e.Description));
            }

            return Result<object>.Created(null, "User created successfully");
        }

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

        public async Task<Result<UserToReturnDto>> UpdateUserAsync(UpdateUserDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.Id);

            if (user == null)
                return Result<UserToReturnDto>.NotFound("User not found!");

            var identityNumber = dto.IdentityNumber?.Trim();

            var exists = await _userManager.Users
                .AnyAsync(u => u.IdentityNumber == identityNumber && u.Id != dto.Id);

            if (exists)
                return Result<UserToReturnDto>
                    .BadRequest("User with that National Id already exists!");

            user.IdentityNumber = identityNumber;
            user.FullName = dto.FullName;
            user.PictureUrl = dto.PictureUrl;
            user.BirthDate = dto.BirthDate;
            user.Address = dto.Address;

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
                return Result<UserToReturnDto>.BadRequest(
                    null,
                    updateResult.Errors.Select(e => e.Description)
                );

            if (!string.IsNullOrWhiteSpace(dto.Role))
            {
                var roleExists = await _roleManager.RoleExistsAsync(dto.Role);
                if (!roleExists)
                    return Result<UserToReturnDto>
                        .BadRequest($"Role '{dto.Role}' does not exist");

                var currentRoles = await _userManager.GetRolesAsync(user);

                if (!currentRoles.Contains(dto.Role))
                {
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    await _userManager.AddToRoleAsync(user, dto.Role);
                }
            }

            var response = await GetUserByIdAsync(user.Id);

            if (!response.Success)
                return response;

            return Result<UserToReturnDto>.Ok(response.Data, "User updated successfully");
        }
        public async Task<Result<UserToReturnDto>> GetUserByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return Result<UserToReturnDto>.NotFound("User not found");

            var userToReturnDto = new UserToReturnDto
            {
                Id = user.Id,
                FullName = user.FullName,
                IdentityNumber = user.IdentityNumber,
                PhoneNumber = user.PhoneNumber,
                PictureUrl = user.PictureUrl,
                BirthDate = user.BirthDate,
                Address = user.Address,
                IsSoftDeleted = user.IsSoftDeleted,
                role = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? ""
            };

            return Result<UserToReturnDto>.Ok(userToReturnDto);
        }
    }
}