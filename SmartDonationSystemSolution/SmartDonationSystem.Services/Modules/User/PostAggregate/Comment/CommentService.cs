using Microsoft.EntityFrameworkCore;
using SmartDonationSystem.Core.Common.Models;
using SmartDonationSystem.Core.Modules.User.PostAggregate.Comment.DTOs;
using SmartDonationSystem.Core.Modules.User.PostAggregate.Comment.Interfaces;
using SmartDonationSystem.DataAccess;
using SmartDonationSystem.Shared.Responses;
using CommentModel = SmartDonationSystem.Core.Common.Models.Comment;
namespace SmartDonationSystem.Services.Modules.User.PostAggregate.Comment
{
    public class CommentService : ICommentService
    {
        private readonly ApplicationDbContext _context;

        public CommentService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Result<CommentDto>> CreateCommentAsync(CreateCommentDto dto, string userId)
        {
            var comment = new CommentModel
            {
                Content = dto.Content,
                PostId = dto.PostId,
                ParentCommentId = dto.ParentCommentId,
                ApplicationUserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();

            // handle mentions
            await HandleTagsAsync(dto.MentionedUserIds, comment.Id);

            // reload comment with mentions
            var commentWithMentions = await _context.Comments
                .Include(c => c.Mentions)
                    .ThenInclude(m => m.MentionedUser)
                .FirstAsync(c => c.Id == comment.Id);

            var user = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => new
                {
                    u.FullName,
                    u.PictureUrl
                })
                .FirstAsync();

            return Result<CommentDto>.Ok(new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                UserName = user.FullName,
                creatorPictureUrl = user.PictureUrl,
                Mentions = commentWithMentions.Mentions.Select(ct => new MentionDto
                {
                    UserId = ct.MentionedUserId,
                    UserName = ct.MentionedUser.FullName
                }).ToList()
            }, "Comment added successfully");
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
                }).ToList();

            await _context.CommentTags.AddRangeAsync(tags);
            await _context.SaveChangesAsync();
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
