using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartDonationSystem.Core.Modules.Payment.DTOs;
using SmartDonationSystem.Core.Modules.Payment.Interfaces;
using System.Security.Claims;

namespace SmartDonationSystem.API.Modules.Payment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentGatewayFactory _factory;
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentGatewayFactory factory, IPaymentService paymentService)
        {
            _factory = factory;
            _paymentService = paymentService;
        }
        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreateDonation(CreateDonationDto dto)
        {
            var DonorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _paymentService.CreateDonationAsync(dto, DonorId);
            return StatusCode((int)result.statusCode, result);
        }

        [HttpGet("my-donations")]
        [Authorize]
        public async Task<IActionResult> GetMyDonations([FromQuery] string? status, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            var donorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _paymentService.GetMyDonationsAsync(donorId, pageNumber, pageSize, status);
            return StatusCode((int)result.statusCode, result);
        }

        [HttpPost("stripe")]
        public async Task<IActionResult> StripeWebhook()
        {
            var payload = await new StreamReader(Request.Body).ReadToEndAsync();
            var signature = Request.Headers["Stripe-Signature"];

            var gateway = _factory.Get("Stripe");
            await gateway.HandleWebhookAsync(payload, signature);

            return Ok();
        }

        [HttpPost("paymob")]
        public async Task<IActionResult> PaymobWebhook()
        {
            var payload = await new StreamReader(Request.Body).ReadToEndAsync();
            var hmac = Request.Query["hmac"].ToString();

            var gateway = _factory.Get("Paymob");
            await gateway.HandleWebhookAsync(payload, hmac);
            return Ok();
        }
    }
}
