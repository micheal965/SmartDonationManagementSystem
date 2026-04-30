using SmartDonationSystem.Core.Modules.Payment.DTOs;
using SmartDonationSystem.Shared.Pagination;
using SmartDonationSystem.Shared.Responses;

namespace SmartDonationSystem.Core.Modules.Payment.Interfaces
{
    public interface IPaymentService
    {
        Task<Result<string>> CreateDonationAsync(CreateDonationDto dto, string DonorId);
        Task<Result<PaginatedList<MyDonationDto>>> GetMyDonationsAsync(string donorId, int pageNumber, int pageSize, string? status = null);
    }
}
