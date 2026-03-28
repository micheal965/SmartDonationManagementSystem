using Mapster;
using Microsoft.EntityFrameworkCore;
using SmartDonationSystem.Core.Common.Models;
using SmartDonationSystem.Core.Modules.Admin.PostManagement.DTOs;
using SmartDonationSystem.Core.Modules.Admin.PostManagement.Interfaces;
using SmartDonationSystem.DataAccess;
using SmartDonationSystem.Shared.Enums;
using SmartDonationSystem.Shared.Pagination;
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

        public async Task<Result<PaginatedList<PostToReturnDto>>> GetPostsAsync(int pageNumber, int pageSize, PostStatus? postStatus)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 10 : pageSize;
            var query = _applicationDbContext.Posts
                                .Include(p => p.Category)
                                .Include(p => p.PostAttachments)
                                .AsQueryable();

            if (postStatus.HasValue)
                query = query.Where(p => p.Status == postStatus.Value.ToString());

            int totalCount = query.Count();
            var posts = await query.OrderByDescending(p => p.CreatedAt).Skip((pageNumber - 1) * pageSize).Take(pageSize)
                  .Select(p => new PostToReturnDto
                  {
                      Id = p.Id,
                      Title = p.Title,
                      Content = p.Content,
                      Status = p.Status,
                      CreatedAt = p.CreatedAt,
                      PostPicture = p.PostPicture,
                      PostAttachments = p.PostAttachments.Select(a => a.AttachmentUrl).ToList(),

                      CategoryName = p.Category.Name,

                      RequesterName = p.ApplicationUser.FullName,
                      RequesterPicture = p.ApplicationUser.PictureUrl
                  })
                  .ToListAsync();
            var PaginatedPosts = new PaginatedList<PostToReturnDto>(posts, pageNumber, pageSize, totalCount);
            return Result<PaginatedList<PostToReturnDto>>.Ok(PaginatedPosts);
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
