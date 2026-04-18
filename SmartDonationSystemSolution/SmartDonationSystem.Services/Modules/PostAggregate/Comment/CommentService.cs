using Microsoft.EntityFrameworkCore;
using SmartDonationSystem.Core.Common.Models;
using SmartDonationSystem.Core.Modules.Notifications.DTOs;
using SmartDonationSystem.Core.Modules.Notifications.Interfaces;
using SmartDonationSystem.Core.Modules.PostAggregate.Comment.DTOs;
using SmartDonationSystem.Core.Modules.PostAggregate.Comment.Interfaces;
using SmartDonationSystem.DataAccess;
using SmartDonationSystem.Shared.Enums;
using SmartDonationSystem.Shared.Responses;
using CommentModel = SmartDonationSystem.Core.Common.Models.Comment;

namespace SmartDonationSystem.Services.Modules.PostAggregate.Comment
{
    public class CommentService : ICommentService
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;

        public CommentService(ApplicationDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }
        public async Task<Result<CommentDto>> CreateCommentAsync(CreateCommentDto dto, string userId)
        {
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return Result<CommentDto>.BadRequest("Invalid user.");

            var post = await _context.Posts.AsNoTracking().FirstOrDefaultAsync(p => p.Id == dto.PostId);
            if (post == null)
                return Result<CommentDto>.BadRequest("Post does not exist.");

            // 1. Create Comment
            var comment = new CommentModel
            {
                Content = dto.Content,
                PostId = dto.PostId,
                ParentCommentId = dto.ParentCommentId,
                ApplicationUserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync(); // needed to generate Comment.Id

            // 2. Handle Mentions (NO SaveChanges here)
            await HandleTagsAsync(dto.MentionedUserIds, comment.Id);

            // 3. Prepare Notification Receivers
            var receivers = dto.MentionedUserIds?
                .Where(id => id != userId)
                .ToHashSet() ?? new HashSet<string>();

            // Add post owner if not self
            if (post.ApplicationUserId != userId)
                receivers.Add(post.ApplicationUserId);

            // 4. Build Notifications
            var notifications = receivers.Select(receiverId =>
            {
                bool isPostOwner = receiverId == post.ApplicationUserId;

                return new Notification
                {
                    ReceiverId = receiverId,
                    ActorId = userId,

                    Title = isPostOwner ? "New Comment" : "New Tag",
                    Message = isPostOwner
                        ? $"{user.FullName} commented on your post"
                        : $"{user.FullName} tagged you on a post",

                    Type = isPostOwner ? NotificationType.Comment : NotificationType.Tag,
                    EntityId = dto.PostId,

                    ActorName = user.FullName,
                    ActorImage = user.PictureUrl
                };
            }).ToList();



            foreach (var notification in notifications)
            {
                await _notificationService.CreateAsync(new CreateNotificationRequest
                {
                    ReceiverId = notification.ReceiverId,
                    ActorId = notification.ReceiverId,
                    Title = notification.Title,
                    Message = notification.Message,
                    Type = notification.Type,
                    EntityId = notification.EntityId,
                    ActorName = notification.ActorName,
                    ActorImage = notification.ActorImage,
                });
            }
            // 6. Build response (NO extra DB call)
            var result = new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                UserName = user.FullName,
                creatorPictureUrl = user.PictureUrl,
                Mentions = dto.MentionedUserIds?
                    .Distinct()
                    .Select(id => new MentionDto
                    {
                        UserId = id
                    }).ToList() ?? new List<MentionDto>()
            };

            return Result<CommentDto>.Ok(result, "Comment added successfully");

        }
        public async Task<Result<List<CommentDto>>> GetPostCommentsAsync(int postId)
        {
            var comments = await _context.Comments
                .Where(c => c.PostId == postId)
                .Include(c => c.ApplicationUser)
                .Include(c => c.Mentions)
                    .ThenInclude(ct => ct.MentionedUser)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            var commentDtos = BuildCommentTree(comments);

            foreach (var c in commentDtos)
            {
                var original = comments.FirstOrDefault(x => x.Id == c.Id);
                if (original != null && original.Mentions != null)
                {
                    c.Mentions = original.Mentions.Select(ct => new MentionDto
                    {
                        UserId = ct.MentionedUserId,
                        UserName = ct.MentionedUser.FullName
                    }).ToList();
                }
            }

            return Result<List<CommentDto>>.Ok(commentDtos, "Comments retrieved successfully");
        }
        public async Task<Result<object>> DeleteCommentAsync(int commentId, string userId)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentId);

            if (comment == null) return Result<object>.NotFound("Comment not found");

            if (comment.ApplicationUserId != userId) return Result<object>.Unauthorized("You cannot delete this comment");

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return Result<object>.NoContent("Comment deleted successfully");
        }
        public async Task<Result<object>> UpdateCommentAsync(int commentId, UpdateCommentDto dto, string userId)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentId);

            if (comment == null) return Result<object>.NotFound("Comment not found");

            if (comment.ApplicationUserId != userId) return Result<object>.Unauthorized("You cannot edit this comment");

            comment.Content = dto.Content;
            await _context.SaveChangesAsync();

            await HandleTagsAsync(dto.MentionedUserIds, comment.Id);

            return Result<object>.NoContent("Comment updated successfully");
        }

        // Helpers
        private async Task HandleTagsAsync(List<string> mentionedUserIds, int commentId)
        {
            if (mentionedUserIds == null || !mentionedUserIds.Any())
                return;

            var tags = mentionedUserIds
                .Distinct()
                .Select(userId => new CommentTag
                {
                    CommentId = commentId,
                    MentionedUserId = userId
                });

            await _context.CommentTags.AddRangeAsync(tags);
        }
        private List<CommentDto> BuildCommentTree(List<CommentModel> comments)
        {
            var commentDtos = comments.Select(c => new CommentDto
            {
                Id = c.Id,
                Content = c.Content,
                UserName = c.ApplicationUser.FullName,
                creatorPictureUrl = c.ApplicationUser.PictureUrl,
                CreatedAt = c.CreatedAt,
                Replies = new List<CommentDto>()
            }).ToList();

            var lookup = commentDtos.ToDictionary(c => c.Id);

            List<CommentDto> roots = new();

            foreach (var comment in comments)
            {
                if (comment.ParentCommentId == null)
                    roots.Add(lookup[comment.Id]);
                else if (lookup.ContainsKey(comment.ParentCommentId.Value))
                    lookup[comment.ParentCommentId.Value].Replies.Add(lookup[comment.Id]);
            }
            return roots;
        }
    }
}
