using SmartDonationSystem.Shared.Responses;

namespace SmartDonationSystem.Core.Modules.User.PostAggregate.Reaction.interfaces
{
    public interface IReactionService
    {
        Task<Result<object>> ReactToPostAsync(string userId, int postId);
    }
}
