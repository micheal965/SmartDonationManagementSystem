using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartDonationSystem.Core.Modules.PostAggregate.Post.DTOs;
using SmartDonationSystem.Core.Modules.PostAggregate.Post.Interfaces;
using System.Security.Claims;

namespace SmartDonationSystem.API.Modules.User.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpPost("create-post")]
        public async Task<IActionResult> CreatePost([FromForm] CreatePostDto createPostDto)
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _postService.CreatePostAsync(createPostDto, userId);
            return StatusCode((int)result.statusCode, result);
        }
        [HttpGet("get-posts")]
        public async Task<IActionResult> GetPosts([FromQuery] PostQueryParams postQueryParams)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _postService
                .GetPostsAsync(userId, postQueryParams.pageNumber, postQueryParams.pageSize, postQueryParams.categoryName, postQueryParams.sortBy);
            return StatusCode((int)result.statusCode, result);
        }
        [HttpGet("get-post/{postId}")]
        public async Task<IActionResult> GetPost(int postId)
        {
            var result = await _postService.GetPostAsync(postId);
            return StatusCode((int)result.statusCode, result);
        }
        [HttpPost("track-post/{postId}")]
        public async Task<IActionResult> TrackPost(int postId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _postService.TrackPostViewAsync(userId, postId);
            return StatusCode((int)result.statusCode);
        }

    }
}
