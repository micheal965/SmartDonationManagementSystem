using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartDonationSystem.Core.Modules.Admin.PostManagement.Interfaces;
using SmartDonationSystem.Shared.Enums;
using System.Security.Claims;

namespace SmartDonationSystem.API.Modules.Admin.Controllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = AppRoles.Admin)]
    public class PostManagementController : ControllerBase
    {
        private readonly IPostManagementService _postService;
        public PostManagementController(IPostManagementService postService)
        {
            _postService = postService;
        }
        [HttpGet("posts")]
        public async Task<IActionResult> GetPosts([FromQuery] PostStatus? postStatus, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _postService.GetPostsAsync(pageNumber, pageSize, postStatus);
            return StatusCode((int)result.statusCode, result);
        }
        [HttpPatch("update-post-status")]
        public async Task<IActionResult> UpdatePostStatus([FromQuery] int postId, [FromQuery] string action)
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _postService.ApproveOrFreezePostAsync(postId, action, userId);
            return StatusCode((int)result.statusCode, result);
        }
        [HttpGet("post-details")]
        public async Task<IActionResult> GetPostById([FromQuery] int id)
        {
            var result = await _postService.GetPostByIdAsync(id);
            return StatusCode((int)result.statusCode, result);
        }
    }
}
