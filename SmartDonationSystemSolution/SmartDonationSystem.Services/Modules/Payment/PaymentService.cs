using Microsoft.EntityFrameworkCore;
using SmartDonationSystem.Core.Common.Models;
using SmartDonationSystem.Core.Modules.Payment.DTOs;
using SmartDonationSystem.Core.Modules.Payment.Interfaces;
using SmartDonationSystem.DataAccess;
using SmartDonationSystem.Shared.Enums;
using SmartDonationSystem.Shared.Pagination;
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
            if (dto.Amount < 50)
                throw new Exception("Invalid amount");

            var donation = new Donation
            {
                Amount = dto.Amount,
                PostId = dto.PostId,
                PaymentGateway = dto.Gateway,
                Type = dto.PostId.HasValue ? DonationType.Post.ToString() : DonationType.Platform.ToString(),
                DonorId = DonorId
            };

            await _context.Donations.AddAsync(donation);
            await _context.SaveChangesAsync();

            var gateway = _factory.Get(dto.Gateway);

            var result = await gateway.CreateCheckoutAsync(donation);

            return result;
        }

        public async Task<Result<PaginatedList<MyDonationDto>>> GetMyDonationsAsync(string donorId, int pageNumber, int pageSize, string? status = null)
        {
            var query = _context.Donations
                .Include(d => d.Post)
                .Where(d => d.DonorId == donorId)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(d => d.Status == status);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(d => d.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(d => new MyDonationDto
                {
                    Id = d.Id,
                    Amount = d.Amount,
                    Status = d.Status,
                    Type = d.Type,
                    PaymentGateway = d.PaymentGateway,
                    PostId = d.PostId,
                    PostTitle = d.Post != null ? d.Post.Title : null,
                    PostPicture = d.Post != null ? d.Post.PostPicture : null,
                    CreatedAt = d.CreatedAt,
                    CheckoutUrl = d.Status == DonationStatus.Pending.ToString() ? d.CheckoutUrl : null
                })
                .ToListAsync();

            var paginatedList = new PaginatedList<MyDonationDto>(items, pageNumber, pageSize, totalCount);

            return Result<PaginatedList<MyDonationDto>>.Ok(paginatedList);
        }
    }
}
