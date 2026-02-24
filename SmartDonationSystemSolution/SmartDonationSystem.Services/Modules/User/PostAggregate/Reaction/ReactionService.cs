using Microsoft.EntityFrameworkCore;
using SmartDonationSystem.Core.Modules.User.PostAggregate.Reaction;
using SmartDonationSystem.DataAccess;
using SmartDonationSystem.Shared.Responses;
using React = SmartDonationSystem.Core.Common.Models.Reaction;

namespace SmartDonationSystem.Services.Modules.User.PostAggregate.Reaction
{
    public class ReactionService : IReactionService
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public ReactionService(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
        public async Task<Result<object>> ReactToPostAsync(string userId, int postId)
        {
            var userExists = await _applicationDbContext.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
                return Result<object>.BadRequest("Invalid user.");

            var postExists = await _applicationDbContext.Posts.AnyAsync(p => p.Id == postId);
            if (!postExists)
                return Result<object>.BadRequest("Post does not exist.");

            var existingReaction = await _applicationDbContext.Reactions
                .FirstOrDefaultAsync(r => r.PostId == postId && r.ApplicationUserId == userId);

            if (existingReaction != null)
                _applicationDbContext.Reactions.Remove(existingReaction);
            else
                await _applicationDbContext.Reactions.AddAsync(new React
                {
                    PostId = postId,
                    ApplicationUserId = userId,
                    CreatedAt = DateTime.UtcNow
                });

            await _applicationDbContext.SaveChangesAsync();

            return Result<object>.Ok("Reaction updated successfully.");
        }
    }
}
