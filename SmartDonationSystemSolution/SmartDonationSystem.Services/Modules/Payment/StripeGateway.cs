using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using SmartDonationSystem.Core.Common.Models;
using SmartDonationSystem.Core.Modules.Notifications.Interfaces;
using SmartDonationSystem.Core.Modules.Payment.Abstractions;
using SmartDonationSystem.Core.Modules.Payment.Interfaces;
using SmartDonationSystem.DataAccess;
using SmartDonationSystem.Shared.Enums;
using SmartDonationSystem.Shared.Responses;
using Stripe;
using Stripe.Checkout;

namespace SmartDonationSystem.Services.Modules.Payment
{
    public class StripeGateway : PaymentNotify, IPaymentGateway
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public string Name => "Stripe";

        public StripeGateway(UserManager<ApplicationUser> userManager, ApplicationDbContext context, INotificationService notificationService, IConfiguration configuration)
            : base(userManager, context, notificationService)
        {
            _context = context;
            _configuration = configuration;
        }
        public async Task<Result<string>> CreateCheckoutAsync(Donation donation)
        {
            var options = new SessionCreateOptions
            {
                Mode = "payment",
                SuccessUrl = $"http://localhost:4200/feed",
                CancelUrl = $"http://localhost:4200/feed",
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        Quantity = 1,
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "egp",
                            UnitAmount = Convert.ToInt64(Math.Round(donation.Amount * 100)),

                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Donation"
                            }
                        }
                    }
                 },
                Metadata = new Dictionary<string, string>
                {
                    { "donationId", donation.Id.ToString() },
                    { "postId", donation.PostId?.ToString() ?? ""}
                }
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);

            return Result<string>.Ok(session.Url);
        }

        public async Task HandleWebhookAsync(string payload, string signature)
        {
            var webhookSecret = _configuration["Payments:Stripe:WebhookSecret"];

            Event stripeEvent;

            try
            {
                stripeEvent = EventUtility.ConstructEvent(
                    payload,
                    signature,
                    webhookSecret
                );
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            // 2. Handle event types
            switch (stripeEvent.Type)
            {
                case "checkout.session.completed":
                    await HandleCheckoutCompleted(stripeEvent);
                    break;

                case "payment_intent.payment_failed":
                    await HandlePaymentFailed(stripeEvent);
                    break;
                default:
                    break;
            }
        }

        private async Task HandleCheckoutCompleted(Event stripeEvent)
        {
            var session = stripeEvent.Data.Object as Stripe.Checkout.Session;

            if (session == null)
                return;

            if (!session.Metadata.TryGetValue("donationId", out var donationIdStr))
                return;

            var donation = await _context.Donations.FindAsync(int.Parse(donationIdStr));

            if (donation == null)
                return;

            if (donation.Status == DonationStatus.Paid.ToString())
                return;

            donation.Status = DonationStatus.Paid.ToString();

            await _context.SaveChangesAsync();

            await NotifyDonationPaidAsync(donation.Id);
        }
        private async Task HandlePaymentFailed(Event stripeEvent)
        {
            var intent = stripeEvent.Data.Object as PaymentIntent;

            if (intent == null)
                return;

            if (!intent.Metadata.TryGetValue("donationId", out var donationIdStr))
                return;


            var donation = await _context.Donations.FindAsync(int.Parse(donationIdStr));

            if (donation == null)
                return;

            donation.Status = DonationStatus.Failed.ToString();

            _context.Donations.Update(donation);
            await _context.SaveChangesAsync();
        }
    }
}
