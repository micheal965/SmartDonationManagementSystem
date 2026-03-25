using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartDonationSystem.Core.Modules.Admin.DashboardManagement.Interfaces;
using SmartDonationSystem.Shared.Enums;

namespace SmartDonationSystem.API.Modules.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = AppRoles.Admin)]
    public class DashboardManagementController(IDashboardManagementService _dashboardService) : ControllerBase
    {
        [HttpGet("last-30-days")]
        public async Task<IActionResult> GetLast30Days()
        {
            var result = await _dashboardService.GetLast30DaysAsync();
            return StatusCode((int)result.statusCode, result);
        }
    }
}
