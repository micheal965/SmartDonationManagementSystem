using SmartDonationSystem.Core.Common.Models;
using SmartDonationSystem.Shared.Responses;

namespace SmartDonationSystem.Core.Modules.Payment.Interfaces
{
    public interface IPaymentGateway
    {
        string Name { get; }

        Task<Result<string>> CreateCheckoutAsync(Donation donation);

        Task HandleWebhookAsync(string payload, string signature);
    }
}
