using Mapster;
using Microsoft.EntityFrameworkCore;
using SmartDonationSystem.Core.Common.Models;
using SmartDonationSystem.Core.Modules.Admin.PostManagement.DTOs;
using SmartDonationSystem.Core.Modules.Admin.PostManagement.Interfaces;
using SmartDonationSystem.DataAccess;
using SmartDonationSystem.Shared.Enums;
using SmartDonationSystem.Shared.Responses;

namespace SmartDonationSystem.Services.Modules.Admin
{
    public class PostManagementService : IPostManagementService
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public PostManagementService(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<Result<List<PostToReturnDto>>> GetPendingAndFreezedPostsAsync()
        {
            var filteredPosts = await _applicationDbContext.Posts
                                .Include(p => p.Category)
                                .Include(p => p.PostAttachments)
                                .Where(p => p.Status == PostStatus.Freezed.ToString() || p.Status == PostStatus.Pending.ToString())
                                .ToListAsync();

            return Result<List<PostToReturnDto>>.Ok(filteredPosts.Adapt<List<PostToReturnDto>>());
        }
        public async Task<Result<object>> ApproveOrFreezePostAsync(int postId, string action, string applicationUserId)
        {
            var post = await _applicationDbContext.Posts.FindAsync(postId);
            if (post == null) return Result<object>.NotFound("Post not found to update");

            if (!Enum.TryParse<PostStatus>(action, true, out var newStatus))
                return Result<object>.BadRequest("Invalid action. Use 'Approved' or 'Freezed'.");

            post.Status = newStatus.ToString();

            if (newStatus == PostStatus.Freezed)
            {
                ApplicationUser? admin = await _applicationDbContext.Users.FindAsync(applicationUserId);
                post.FreezedBy = admin?.FullName ?? admin?.UserName ?? "Unknown Admin";
            }

            await _applicationDbContext.SaveChangesAsync();
            return Result<object>.Ok("Post status Updated successfully");
        }

    }
}
