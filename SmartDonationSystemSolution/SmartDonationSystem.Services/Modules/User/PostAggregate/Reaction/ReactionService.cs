using Microsoft.EntityFrameworkCore;
using SmartDonationSystem.Core.Modules.Notifications.DTOs;
using SmartDonationSystem.Core.Modules.Notifications.Interfaces;
using SmartDonationSystem.Core.Modules.User.PostAggregate.Reaction.interfaces;
using SmartDonationSystem.DataAccess;
using SmartDonationSystem.Shared.Enums;
using SmartDonationSystem.Shared.Responses;
using React = SmartDonationSystem.Core.Common.Models.Reaction;

namespace SmartDonationSystem.Services.Modules.User.PostAggregate.Reaction
{
    public class ReactionService : IReactionService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly INotificationService _notificationService;

        public ReactionService(ApplicationDbContext applicationDbContext, INotificationService notificationService)
        {
            _applicationDbContext = applicationDbContext;
            _notificationService = notificationService;
        }
        public async Task<Result<object>> ReactToPostAsync(string userId, int postId)
        {
            var user = await _applicationDbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return Result<object>.BadRequest("Invalid user.");

            var post = await _applicationDbContext.Posts.FirstOrDefaultAsync(p => p.Id == postId);
            if (post == null)
                return Result<object>.BadRequest("Post does not exist.");

            var existingReaction = await _applicationDbContext.Reactions
                .FirstOrDefaultAsync(r => r.PostId == postId && r.ApplicationUserId == userId);

            if (existingReaction != null)
                _applicationDbContext.Reactions.Remove(existingReaction);
            else
            {
                await _applicationDbContext.Reactions.AddAsync(new React
                {
                    PostId = postId,
                    ApplicationUserId = userId,
                    CreatedAt = DateTime.UtcNow
                });
                // 2. Trigger notification
                await _notificationService.CreateAsync(new CreateNotificationRequest
                {
                    ReceiverId = post.ApplicationUserId,
                    ActorId = userId,

                    Title = "New Like",
                    Message = $"{user.FullName} liked your post",

                    Type = NotificationType.Like,
                    EntityId = postId,

                    ActorName = user.FullName,
                    ActorImage = user.PictureUrl
                });
            }
            await _applicationDbContext.SaveChangesAsync();

            return Result<object>.Ok("Reaction updated successfully.");
        }
    }
}
