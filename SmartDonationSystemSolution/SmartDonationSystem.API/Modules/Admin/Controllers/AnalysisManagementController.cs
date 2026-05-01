using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartDonationSystem.Core.Modules.Admin.AnalysisManagement.Interfaces;
using SmartDonationSystem.Shared.Enums;

namespace SmartDonationSystem.API.Modules.Admin.Controllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = AppRoles.Admin)]

    public class AnalysisManagementController : ControllerBase
    {
        private readonly IAnalysisManagementService _analysisService;

        public AnalysisManagementController(IAnalysisManagementService analysisService)
        {
            _analysisService = analysisService;
        }
        [HttpGet("analysis")]
        public async Task<IActionResult> GetAnalysis([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            var result = await _analysisService.GetAnalysisDataAsync(fromDate, toDate);
            return StatusCode((int)result.statusCode, result);
        }
    }
}
