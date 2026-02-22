using Hangfire;
using Mapster;
using Microsoft.EntityFrameworkCore;
using SmartDonationSystem.Core.Common.Models;
using SmartDonationSystem.Core.Modules.Cloud;
using SmartDonationSystem.Core.Modules.User.PostAggregate.Post.DTOs;
using SmartDonationSystem.Core.Modules.User.PostAggregate.Post.Interfaces;
using SmartDonationSystem.DataAccess;
using SmartDonationSystem.Services.Modules.AI.SummarizationModule;
using SmartDonationSystem.Services.Modules.FileExtractionModule;
using SmartDonationSystem.Shared.Pagination;
using SmartDonationSystem.Shared.Responses;
using PostModel = SmartDonationSystem.Core.Common.Models.Post;
using PostToReturnDto = SmartDonationSystem.Core.Modules.User.PostAggregate.Post.DTOs.PostToReturnDto;

namespace SmartDonationSystem.Services.Modules.User.PostAggregate.Post
{
    public class PostService : IPostService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ICloudinaryServices _cloudinaryServices;

        public PostService(ApplicationDbContext applicationDbContext, ICloudinaryServices cloudinaryServices)
        {
            _applicationDbContext = applicationDbContext;
            _cloudinaryServices = cloudinaryServices;
        }
        public async Task<Result<object>> CreatePostAsync(CreatePostDto createPostDto, string applicationUserId)
        {
            ApplicationUser? User = await _applicationDbContext.Users.FindAsync(applicationUserId);
            if (User == null)
                return Result<object>.BadRequest("User not found");

            bool categoryExists = await _applicationDbContext.Categories.AnyAsync(c => c.Id == createPostDto.categoryId);
            if (!categoryExists)
                return Result<object>.BadRequest("Category not found");

            var attachmentsUrls = await _cloudinaryServices.UploadFilesAsync(createPostDto.attachments, "post_attachments");
            if (attachmentsUrls == null)
                return Result<object>.ServerError("Failed to upload attachments");

            PostModel post = new PostModel
            {
                ApplicationUserId = applicationUserId,
                Title = createPostDto.title,
                Content = createPostDto.content,
                CategoryId = createPostDto.categoryId,
                PostAttachments = attachmentsUrls.Select(url => new PostAttachment
                {
                    AttachmentUrl = url
                }).ToList()
            };

            await _applicationDbContext.Posts.AddAsync(post);
            await _applicationDbContext.SaveChangesAsync();


            // Step 1: Enqueue extraction
            var jobId = BackgroundJob.Enqueue<FileExtractionJob>(job => job.ExtractAndSaveTextAsync(post.Id));

            // Step 2: Continue with summary job (post.Id serializable)
            BackgroundJob.ContinueJobWith<SummaryJob>(
                jobId,
                job => job.GenerateSummaryAsync(post.Id)
            );

            return Result<object>.Created(null, "Post created successfully");
        }

        public async Task<Result<PaginatedList<PostToReturnDto>>> GetPostsAsync(int pageNumber, int pageSize)
        {
            var query = _applicationDbContext.Posts
            //                    .Where(p => p.Status == PostStatus.Approved.ToString() && p.PriorityLevel != null && p.ImpactScore != null)
                                .OrderByDescending(p => p.PriorityLevel)
                                .ThenByDescending(p => p.ImpactScore)
                                .ThenByDescending(p => p.CreatedAt)
                                .Select(p => new PostToReturnDto()
                                {
                                    id = p.Id,
                                    title = p.Title,
                                    content = p.Content,
                                    createdAt = p.CreatedAt,
                                    priorityLevel = p.PriorityLevel,
                                    userId = p.ApplicationUserId,
                                    fullName = p.ApplicationUser.FullName,
                                    attachments = p.PostAttachments.Select(pa => pa.AttachmentUrl).ToList(),
                                    pictureUrl = p.ApplicationUser.PictureUrl
                                });

            var totalCount = await query.CountAsync();

            var posts = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var postsToReturnDto = posts.Adapt<List<PostToReturnDto>>();

            var result = new PaginatedList<PostToReturnDto>(postsToReturnDto, pageNumber, pageSize, totalCount);
            return Result<PaginatedList<PostToReturnDto>>.Ok(result);
        }
    }
}
