using SmartDonationSystem.Core.Modules.Payment.DTOs;
using SmartDonationSystem.Shared.Responses;

namespace SmartDonationSystem.Core.Modules.Payment.Interfaces
{
    public interface IPaymentService
    {
        Task<Result<string>> CreateDonationAsync(CreateDonationDto dto, string DonorId);
    }
}
