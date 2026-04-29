using SmartDonationSystem.Core.Common.Models;
using SmartDonationSystem.Core.Modules.Payment.DTOs;
using SmartDonationSystem.Core.Modules.Payment.Interfaces;
using SmartDonationSystem.DataAccess;
using SmartDonationSystem.Shared.Enums;
using SmartDonationSystem.Shared.Responses;

namespace SmartDonationSystem.Services.Modules.Payment
{
    public class PaymentService : IPaymentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPaymentGatewayFactory _factory;

        public PaymentService(ApplicationDbContext context, IPaymentGatewayFactory factory)
        {
            _context = context;
            _factory = factory;
        }

        public async Task<Result<string>> CreateDonationAsync(CreateDonationDto dto, string DonorId)
        {
            if (dto.Amount <= 0)
                throw new Exception("Invalid amount");

            var donation = new Donation
            {
                Amount = dto.Amount,
                PostId = dto.PostId,
                PaymentGateway = dto.Gateway,
                Type = dto.PostId.HasValue ? DonationType.Post : DonationType.Platform,
                DonorId = DonorId
            };

            await _context.Donations.AddAsync(donation);
            await _context.SaveChangesAsync();

            var gateway = _factory.Get(dto.Gateway);

            var result = await gateway.CreateCheckoutAsync(donation);

            return result;
        }
    }
}
