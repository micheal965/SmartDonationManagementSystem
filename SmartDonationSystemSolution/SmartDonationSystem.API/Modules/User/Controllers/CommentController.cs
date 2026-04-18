using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartDonationSystem.Core.Modules.PostAggregate.Comment.DTOs;
using SmartDonationSystem.Core.Modules.PostAggregate.Comment.Interfaces;
using System.Security.Claims;

namespace SmartDonationSystem.API.Modules.User.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpPost("create-comment")]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentDto dto)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _commentService.CreateCommentAsync(dto, userId);

            return StatusCode((int)result.statusCode, result);
        }

        [HttpGet("get-post-comments/{postId}")]
        public async Task<IActionResult> GetPostComments(int postId)
        {
            var result = await _commentService.GetPostCommentsAsync(postId);
            return StatusCode((int)result.statusCode, result);
        }

        [HttpPut("update-comment/{id}")]
        public async Task<IActionResult> UpdateComment(int id, [FromBody] UpdateCommentDto dto)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _commentService.UpdateCommentAsync(id, dto, userId);

            return StatusCode((int)result.statusCode, result);
        }

        [HttpDelete("delete-comment/{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _commentService.DeleteCommentAsync(id, userId);

            return StatusCode((int)result.statusCode, result);
        }
    }
}
