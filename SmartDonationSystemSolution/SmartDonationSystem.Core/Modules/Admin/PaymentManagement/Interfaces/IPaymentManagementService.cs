using SmartDonationSystem.Core.Modules.Admin.PaymentManagement.DTOs;
using SmartDonationSystem.Shared.Pagination;
using SmartDonationSystem.Shared.Responses;

namespace SmartDonationSystem.Core.Modules.Admin.PaymentManagement.Interfaces
{
    public interface IPaymentManagementService
    {
        Task<Result<PaginatedList<DonationToReturnDto>>> GetDonationsAsync(int pageNumber, int pageSize, string? status = null);
        Task<Result<DonationDetailsDto>> GetDonationByIdAsync(int id);
        Task<Result<object>> ApproveDonationAsync(int id);
        Task<Result<decimal>> GetTotalCollectedAmountAsync();
    }
}
