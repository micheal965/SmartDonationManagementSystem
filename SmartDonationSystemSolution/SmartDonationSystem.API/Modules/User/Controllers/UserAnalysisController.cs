using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartDonationSystem.Core.Modules.User.UserAnalysis.Interfaces;

namespace SmartDonationSystem.API.Modules.User.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserAnalysisController : ControllerBase
    {
        private readonly IUserAnalysisService _userAnalysisService;

        public UserAnalysisController(IUserAnalysisService userAnalysisService)
        {
            _userAnalysisService = userAnalysisService;
        }

        [HttpGet("my-impact")]
        public async Task<IActionResult> GetMyImpact()
        {
            var result = await _userAnalysisService.GetUserAnalysisAsync();
            return StatusCode((int)result.statusCode, result);
        }
    }
}
