using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartDonationSystem.Core.Modules.Admin.PaymentManagement.Interfaces;
using SmartDonationSystem.Shared.Enums;

namespace SmartDonationSystem.API.Modules.Admin.Controllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = AppRoles.Admin)]
    public class PaymentManagementController(IPaymentManagementService _paymentManagementService) : ControllerBase
    {
        [HttpGet("get-donations")]
        public async Task<IActionResult> GetDonations([FromQuery] string? status, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            var result = await _paymentManagementService.GetDonationsAsync(pageNumber, pageSize, status);
            return StatusCode((int)result.statusCode, result);
        }

        [HttpGet("get-donation/{id}")]
        public async Task<IActionResult> GetDonationById(int id)
        {
            var result = await _paymentManagementService.GetDonationByIdAsync(id);
            return StatusCode((int)result.statusCode, result);
        }

        [HttpPost("approve/{id}")]
        public async Task<IActionResult> ApproveDonation(int id)
        {
            var result = await _paymentManagementService.ApproveDonationAsync(id);
            return StatusCode((int)result.statusCode, result);
        }

        [HttpGet("total-collected")]
        public async Task<IActionResult> GetTotalCollectedAmount()
        {
            var result = await _paymentManagementService.GetTotalCollectedAmountAsync();
            return StatusCode((int)result.statusCode, result);
        }
    }
}
