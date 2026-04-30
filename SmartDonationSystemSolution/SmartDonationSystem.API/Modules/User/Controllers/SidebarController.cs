using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartDonationSystem.Core.Modules.User.Sidebar.Interfaces;

namespace SmartDonationSystem.API.Modules.User.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SidebarController : ControllerBase
    {
        private readonly ISidebarService _sidebarService;

        public SidebarController(ISidebarService sidebarService)
        {
            _sidebarService = sidebarService;
        }

        [HttpGet("data")]
        public async Task<IActionResult> GetSidebarData()
        {
            var result = await _sidebarService.GetSidebarDataAsync();
            return StatusCode((int)result.statusCode, result);
        }
    }
}