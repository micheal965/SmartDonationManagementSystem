using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartDonationSystem.Core.Modules.Admin.ReportManagement.DTOs;
using SmartDonationSystem.Core.Modules.Admin.ReportManagement.Interfaces;
using SmartDonationSystem.Shared.Enums;

namespace SmartDonationSystem.API.Modules.Admin.Controllers
{
    [ApiController]
    [Route("api/admin/ReportManagement")]
    [Authorize(Roles = AppRoles.Admin)]
    public class ReportManagementController(
        IReportService _reportService, 
        Microsoft.AspNetCore.Hosting.IWebHostEnvironment _env) : ControllerBase
    {
        [HttpPost("pdf")]
        public async Task<IActionResult> GeneratePdf([FromBody] ReportRequest request)
        {
            var logoPath = System.IO.Path.Combine(_env.WebRootPath, "assets", "logo.png");
            var result = await _reportService.GeneratePdfAsync(request, logoPath);

            if (!result.Success)
            {
                return StatusCode((int)result.statusCode, result);
            }

            var fileName = $"{request.ReportType}_{System.DateTime.UtcNow:yyyyMMddHHmm}.pdf";
            return File(result.Data, "application/pdf", fileName);
        }
    }
}
