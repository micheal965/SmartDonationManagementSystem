//using SmartDonationSystem.Core.Common.Models;
//using SmartDonationSystem.Core.Modules.Payment.Interfaces;
//using SmartDonationSystem.Shared.Responses;

//namespace SmartDonationSystem.Services.Modules.Payment
//{
//    public class PaymobGateway : IPaymentGateway
//    {
//        public string Name => "Paymob";

//        public async Task<Result<string>> CreateCheckoutAsync(Donation donation)
//        {
//            // Paymob iframe / payment key logic
//            return "paymob_iframe_url";
//        }

//        public async Task HandleWebhookAsync(string payload, string signature)
//        {
//            // Paymob webhook verification
//        }
//    }
//}
