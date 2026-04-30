using Microsoft.EntityFrameworkCore;
using SmartDonationSystem.Core.Modules.Admin.PaymentManagement.DTOs;
using SmartDonationSystem.Core.Modules.Admin.PaymentManagement.Interfaces;
using SmartDonationSystem.Core.Modules.Notifications.DTOs;
using SmartDonationSystem.Core.Modules.Notifications.Interfaces;
using SmartDonationSystem.DataAccess;
using SmartDonationSystem.Shared.Enums;
using SmartDonationSystem.Shared.Pagination;
using SmartDonationSystem.Shared.Responses;

namespace SmartDonationSystem.Services.Modules.Admin.PaymentManagement
{
    public class PaymentManagementService(ApplicationDbContext _context, INotificationService _notificationService) : IPaymentManagementService
    {
        public async Task<Result<PaginatedList<DonationToReturnDto>>> GetDonationsAsync(int pageNumber, int pageSize, string? status = null)
        {
            var query = _context.Donations
                .Include(d => d.Donor)
                .Include(d => d.Post)
                    .ThenInclude(p => p != null ? p.ApplicationUser : null)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(d => d.Status == status);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(d => d.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(d => new DonationToReturnDto
                {
                    Id = d.Id,
                    Amount = d.Amount,
                    Status = d.Status,
                    Type = d.Type,
                    PaymentGateway = d.PaymentGateway,
                    PostId = d.PostId,
                    PostTitle = d.Post != null ? d.Post.Title : null,
                    DonorId = d.DonorId,
                    DonorName = d.Donor.FullName,
                    DonorPhoneNumber = d.Donor.PhoneNumber,
                    RequesterPhoneNumber = d.Post != null ? d.Post.ApplicationUser.PhoneNumber : null,
                    CreatedAt = d.CreatedAt
                })
                .ToListAsync();

            var paginatedList = new PaginatedList<DonationToReturnDto>(items, pageNumber, pageSize, totalCount);

            return Result<PaginatedList<DonationToReturnDto>>.Ok(paginatedList);

        }

        public async Task<Result<DonationDetailsDto>> GetDonationByIdAsync(int id)
        {
            var donation = await _context.Donations
                .Include(d => d.Donor)
                .Include(d => d.Post)
                    .ThenInclude(p => p != null ? p.ApplicationUser : null)
                .Include(d => d.Post)
                    .ThenInclude(p => p != null ? p.Category : null)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (donation == null)
                return Result<DonationDetailsDto>.NotFound("Donation not found.");

            var dto = new DonationDetailsDto
            {
                Id = donation.Id,
                Amount = donation.Amount,
                Status = donation.Status,
                Type = donation.Type,
                PaymentGateway = donation.PaymentGateway,
                PostId = donation.PostId,
                PostTitle = donation.Post?.Title,
                PostPicture = donation.Post?.PostPicture,
                CategoryName = donation.Post?.Category?.Name,
                DonorId = donation.DonorId,
                DonorName = donation.Donor?.FullName ?? "Unknown",
                DonorPhoneNumber = donation.Donor?.PhoneNumber ?? "Unknown",
                RequesterName = donation.Post?.ApplicationUser?.FullName,
                RequesterPhoneNumber = donation.Post?.ApplicationUser?.PhoneNumber,
                CreatedAt = donation.CreatedAt
            };

            return Result<DonationDetailsDto>.Ok(dto);
        }

        public async Task<Result<object>> ApproveDonationAsync(int id)
        {
            var donation = await _context.Donations
                .Include(d => d.Donor)
                .Include(d => d.Post)
                    .ThenInclude(p => p != null ? p.ApplicationUser : null)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (donation == null)
                return Result<object>.NotFound("Donation not found.");

            if (donation.Status != DonationStatus.Paid.ToString())
                return Result<object>.BadRequest("Only paid donations can be approved.");

            donation.Status = DonationStatus.Processed.ToString();
            await _context.SaveChangesAsync();

            // Notify Donor
            await _notificationService.CreateAsync(new CreateNotificationRequest
            {
                ReceiverId = donation.DonorId,
                Title = "Donation Processed",
                Message = $"Your donation of {donation.Amount} EGP to \"{donation.Post?.Title ?? "Platform"}\" has been processed and sent to the requester.",
                Type = NotificationType.DonationProcessed,
                EntityId = donation.Id
            });

            // Notify Requester (Post Author)
            if (donation.Post != null && !string.IsNullOrEmpty(donation.Post.ApplicationUserId))
            {
                await _notificationService.CreateAsync(new CreateNotificationRequest
                {
                    ReceiverId = donation.Post.ApplicationUserId,
                    Title = "Donation Received in Wallet",
                    Message = $"A donation of {donation.Amount} EGP from {donation.Donor?.FullName ?? "a donor"} has been sent to your wallet.",
                    Type = NotificationType.DonationReceived,
                    EntityId = (int)donation.PostId
                });
            }

            return Result<object>.Ok(null, "Donation approved and notifications sent successfully.");
        }

        public async Task<Result<decimal>> GetTotalCollectedAmountAsync()
        {
            var total = await _context.Donations
                .Where(d => d.Status == DonationStatus.Paid.ToString() || d.Status == DonationStatus.Processed.ToString())
                .SumAsync(d => d.Amount);

            return Result<decimal>.Ok(total);
        }
    }
}
