using SmartDonationSystem.Core.Modules.User.PostAggregate.Comment.DTOs;
using SmartDonationSystem.Shared.Responses;

namespace SmartDonationSystem.Core.Modules.User.PostAggregate.Comment.Interfaces
{
    public interface ICommentService
    {
        Task<Result<CommentDto>> CreateCommentAsync(CreateCommentDto dto, string userId);
        Task<Result<List<CommentDto>>> GetPostCommentsAsync(int postId);
        Task<Result<object>> DeleteCommentAsync(int commentId, string userId);
        Task<Result<object>> UpdateCommentAsync(int commentId, UpdateCommentDto dto, string userId);
    }
}
