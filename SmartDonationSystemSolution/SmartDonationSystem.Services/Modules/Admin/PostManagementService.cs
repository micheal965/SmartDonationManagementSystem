using Mapster;
using Microsoft.EntityFrameworkCore;
using SmartDonationSystem.Core.Common.Models;
using SmartDonationSystem.Core.Modules.Admin.PostManagement.DTOs;
using SmartDonationSystem.Core.Modules.Admin.PostManagement.Interfaces;
using SmartDonationSystem.Core.Modules.Notifications.DTOs;
using SmartDonationSystem.Core.Modules.Notifications.Interfaces;
using SmartDonationSystem.DataAccess;
using SmartDonationSystem.Shared.Enums;
using SmartDonationSystem.Shared.Pagination;
using SmartDonationSystem.Shared.Responses;

namespace SmartDonationSystem.Services.Modules.Admin
{
    public class PostManagementService : IPostManagementService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly INotificationService _notificationService;

        public PostManagementService(ApplicationDbContext applicationDbContext, INotificationService notificationService)
        {
            _applicationDbContext = applicationDbContext;
            _notificationService = notificationService;
        }

        public async Task<Result<PostToReturnDto>> GetPostByIdAsync(int postId)
        {
            var post = await _applicationDbContext.Posts
                .Include(p => p.Category)
                .Include(p => p.PostAttachments)
                .Include(p => p.ApplicationUser)
                .Where(p => p.Id == postId)
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
                    CreatorName = p.ApplicationUser.FullName,
                    creatorPicture = p.ApplicationUser.PictureUrl,
                    creatorRole = p.CreatedByRole,
                    TargetMoney = p.TargetMoney
                })
                .FirstOrDefaultAsync();

            if (post == null)
                return Result<PostToReturnDto>.NotFound("Post not found");

            return Result<PostToReturnDto>.Ok(post);
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

                      CreatorName = p.ApplicationUser.FullName,
                      creatorPicture = p.ApplicationUser.PictureUrl,
                      creatorRole = p.CreatedByRole,
                      TargetMoney = p.TargetMoney
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

            await _notificationService.CreateAsync(new CreateNotificationRequest
            {
                ReceiverId = post.ApplicationUserId,
                Title = "Post Status Updated",
                Message = action == PostStatus.Approved.ToString() ? "Congratulations! Your post has been approved and is now visible to others."
                             : "Unfortunately, your post has been rejected. Please contact support if you believe this is a mistake.",
                Type = (action == PostStatus.Approved.ToString()) ? NotificationType.PostApproval : NotificationType.PostRejection,
                EntityId = post.Id
            });

            //Notify Users that interesting in that category
            if (action == PostStatus.Approved.ToString())
            {
                var category = await _applicationDbContext.Categories
                    .Where(c => c.Id == post.CategoryId)
                    .Select(c => c.Name)
                    .FirstOrDefaultAsync();

                var targetRole = post.CreatedByRole == "Requester" ? "Donor" : "Requester";

                var userIds = await (
                 from uc in _applicationDbContext.UserCategories
                 join u in _applicationDbContext.Users on uc.UserId equals u.Id
                 join ur in _applicationDbContext.UserRoles on u.Id equals ur.UserId
                 join r in _applicationDbContext.Roles on ur.RoleId equals r.Id
                 where uc.CategoryId == post.CategoryId
                       && uc.UserId != post.ApplicationUserId
                       && r.Name == targetRole
                 select uc.UserId).ToListAsync();

                foreach (var userId in userIds)
                {
                    await _notificationService.CreateAsync(new CreateNotificationRequest
                    {
                        ReceiverId = userId,
                        Title = "New Post Update",
                        Message = $"A new post in '{category}' category is now available. Click to check it out.",
                        Type = NotificationType.PostApproval,
                        EntityId = post.Id
                    });
                }
            }

            return Result<object>.Ok(null, "Post status Updated successfully");
        }
    }
}
