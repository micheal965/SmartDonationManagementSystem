using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartDonationSystem.Core.Modules.Admin.DashboardManagement.Interfaces;
using SmartDonationSystem.Shared.Enums;

namespace SmartDonationSystem.API.Modules.Admin.Controllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = AppRoles.Admin)]
    public class DashboardManagementController(IDashboardManagementService _dashboardService) : ControllerBase
    {
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var result = await _dashboardService.GetDashboardData();
            return StatusCode((int)result.statusCode, result);
        }
    }
}
