using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartDonationSystem.Core.Modules.User.PostAggregate.Reaction;
using System.Security.Claims;

namespace SmartDonationSystem.API.Modules.User.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReactionController : ControllerBase
    {
        private readonly IReactionService _reactionService;

        public ReactionController(IReactionService reactionService)
        {
            _reactionService = reactionService;
        }
        [HttpPost("react")]
        public async Task<IActionResult> ReactToPost([FromQuery] int postId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _reactionService.ReactToPostAsync(userId, postId);
            return StatusCode((int)result.statusCode, result);
        }
    }
}
