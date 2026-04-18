using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartDonationSystem.Core.Common.Models;
using SmartDonationSystem.Core.Modules.Cloud;
using SmartDonationSystem.Core.Modules.Notifications.DTOs;
using SmartDonationSystem.Core.Modules.Notifications.Interfaces;
using SmartDonationSystem.Core.Modules.PostAggregate.Post.DTOs;
using SmartDonationSystem.Core.Modules.PostAggregate.Post.Interfaces;
using SmartDonationSystem.DataAccess;
using SmartDonationSystem.Services.Modules.AI.SummarizationModule;
using SmartDonationSystem.Services.Modules.FileExtractionModule;
using SmartDonationSystem.Shared.Enums;
using SmartDonationSystem.Shared.Pagination;
using SmartDonationSystem.Shared.Responses;
using System.Diagnostics;
using PostModel = SmartDonationSystem.Core.Common.Models.Post;
using PostToReturnDto = SmartDonationSystem.Core.Modules.PostAggregate.Post.DTOs.PostToReturnDto;

namespace SmartDonationSystem.Services.Modules.PostAggregate.Post
{
    public class PostService : IPostService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ICloudinaryServices _cloudinaryServices;
        private readonly INotificationService _notificationService;
        private readonly UserManager<ApplicationUser> _userManager;

        public PostService(UserManager<ApplicationUser> userManager, ApplicationDbContext applicationDbContext, ICloudinaryServices cloudinaryServices, INotificationService notificationService)
        {
            _applicationDbContext = applicationDbContext;
            _cloudinaryServices = cloudinaryServices;
            _notificationService = notificationService;
            _userManager = userManager;
        }
        public async Task<Result<object>> CreatePostAsync(CreatePostDto createPostDto, string applicationUserId)
        {
            ApplicationUser? User = await _applicationDbContext.Users.FindAsync(applicationUserId);
            if (User == null)
                return Result<object>.BadRequest("User not found");

            Category? category = await _applicationDbContext.Categories.FindAsync(createPostDto.categoryId);
            if (category == null)
                return Result<object>.BadRequest("Category not found");

            if (createPostDto.attachments != null && createPostDto.attachments.Count > 5)
                return Result<object>.BadRequest("You can upload a maximum of 5 attachments.");

            if (string.Equals(category.Name, "Medical", StringComparison.OrdinalIgnoreCase) &&
                (createPostDto.attachments == null || !createPostDto.attachments.Any()))
                return Result<object>.BadRequest("At least one attachment is required for Medical category.");

            var PostPictureResult = await _cloudinaryServices.UploadImageAsync(createPostDto.PostPicture);

            PostModel post = new PostModel
            {
                ApplicationUserId = applicationUserId,
                Title = createPostDto.title,
                Content = createPostDto.content,
                CategoryId = createPostDto.categoryId,
                PostPicture = PostPictureResult.url
            };
            if (createPostDto.attachments != null)
            {
                var attachmentsUrls = await _cloudinaryServices.UploadFilesAsync(createPostDto.attachments, "post_attachments");
                if (attachmentsUrls == null)
                    return Result<object>.ServerError("Failed to upload attachments");

                post.PostAttachments = attachmentsUrls.Select(url => new PostAttachment
                {
                    AttachmentUrl = url
                }).ToList();
            }

            await _applicationDbContext.Posts.AddAsync(post);
            await _applicationDbContext.SaveChangesAsync();

            //Notify admins for approval
            await NotifyAdminsPostPendingApproval(post);

            // Step 1: Enqueue extraction
            var jobId = BackgroundJob.Enqueue<FileExtractionJob>(job => job.ExtractAndSaveTextAsync(post.Id));

            // Step 2: Continue with summary job (post.Id serializable)
            BackgroundJob.ContinueJobWith<SummaryJob>(
                jobId,
                job => job.GenerateSummaryAsync(post.Id)
            );

            return Result<object>.Created(null, "Post created successfully");
        }

        public async Task<Result<PaginatedList<PostToReturnDto>>> GetPostsAsync(string userId, int pageNumber, int pageSize, string? categoryName, PostSortBy sortBy)
        {
            if (!Enum.IsDefined(typeof(PostSortBy), sortBy))
                return Result<PaginatedList<PostToReturnDto>>.BadRequest($"Invalid sort type: {sortBy}");

            var query = _applicationDbContext.Posts
                          .Where(p => p.Status == PostStatus.Approved.ToString()
                          && (string.IsNullOrEmpty(categoryName) || p.Category.Name == categoryName));

            query = sortBy switch
            {
                PostSortBy.Urgent => query.OrderByDescending(p => p.PriorityLevel)
                                        .ThenByDescending(p => p.ImpactScore)
                                        .ThenByDescending(p => p.CreatedAt),
                PostSortBy.Recent => query.OrderByDescending(p => p.CreatedAt),
                PostSortBy.MostViewed => query.OrderByDescending(p => p.AnalyticsEvents!.Count())
                                                 .ThenByDescending(p => p.ImpactScore)
                                                  .ThenByDescending(p => p.CreatedAt),
                _ => throw new UnreachableException()
            };


            var totalCount = await query.CountAsync();

            var postsToReturnDto = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PostToReturnDto
                {
                    id = p.Id,
                    title = p.Title,
                    content = p.Content,
                    createdAt = p.CreatedAt,
                    priorityLevel = p.PriorityLevel,
                    attachments = p.PostAttachments.Select(pa => pa.AttachmentUrl).ToList(),
                    likesCount = p.Reactions.Count(),
                    PostPicture = p.PostPicture,
                    hasReacted = p.Reactions.Any(r => r.ApplicationUserId == userId),
                    viewCount = p.AnalyticsEvents!.Count(),
                    userId = p.ApplicationUserId,
                    fullName = p.ApplicationUser.FullName,
                    pictureUrl = p.ApplicationUser.PictureUrl,
                    categoryName = p.Category.Name
                })
                .ToListAsync();

            var result = new PaginatedList<PostToReturnDto>(postsToReturnDto, pageNumber, pageSize, totalCount);
            return Result<PaginatedList<PostToReturnDto>>.Ok(result);
        }
        public async Task<Result<PostToReturnDto>> GetPostAsync(int postId)
        {
            PostToReturnDto? post = await _applicationDbContext.Posts
                .Where(p => p.Id == postId)
                .Select(p => new PostToReturnDto
                {
                    id = p.Id,
                    title = p.Title,
                    content = p.Content,
                    createdAt = p.CreatedAt,
                    priorityLevel = p.PriorityLevel,
                    attachments = p.PostAttachments.Select(pa => pa.AttachmentUrl).ToList(),
                    likesCount = p.Reactions.Count(),
                    viewCount = p.AnalyticsEvents!.Count(),
                    PostPicture = p.PostPicture,
                    userId = p.ApplicationUserId,
                    fullName = p.ApplicationUser.FullName,
                    pictureUrl = p.ApplicationUser.PictureUrl,
                    phoneNumber = p.ApplicationUser.PhoneNumber,
                    categoryName = p.Category.Name
                }).FirstOrDefaultAsync();

            if (post == null)
                return Result<PostToReturnDto>.NotFound("Post not found");

            return Result<PostToReturnDto>.Ok(post);
        }

        public async Task<Result<object>> TrackPostViewAsync(string userId, int postId)
        {
            var exists = await _applicationDbContext.AnalyticsEvents
                    .AnyAsync(x => x.PostId == postId && x.ApplicationUserId == userId);

            if (exists) return Result<object>.NoContent();

            var ev = new AnalyticsEvent
            {
                Type = AnalyticsEventType.PostView,
                PostId = postId,
                ApplicationUserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _applicationDbContext.AnalyticsEvents.AddAsync(ev);
            await _applicationDbContext.SaveChangesAsync();
            return Result<object>.NoContent();
        }

        private async Task NotifyAdminsPostPendingApproval(PostModel post)
        {
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            if (!admins.Any()) return;

            var notificationsRequests = admins.Select(admin => new CreateNotificationRequest
            {
                ReceiverId = admin.Id,
                ActorId = post.ApplicationUserId,
                Title = "New Post Waiting Approval",
                Message = $"{post.ApplicationUser.FullName} created a new post \"{post.Title}\" and needs approval.",
                Type = NotificationType.PostCreation,
                EntityId = post.Id,
                ActorName = post.ApplicationUser.FullName,
                ActorImage = post.ApplicationUser.PictureUrl,
            }).ToList();

            foreach (var notificationRequest in notificationsRequests)
                await _notificationService.CreateAsync(notificationRequest);
        }
    }
}
