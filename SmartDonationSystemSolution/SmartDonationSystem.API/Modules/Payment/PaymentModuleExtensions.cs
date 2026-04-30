using SmartDonationSystem.Core.Modules.Payment.Interfaces;
using SmartDonationSystem.Services.Modules.Payment;

namespace SmartDonationSystem.API.Modules.Payment
{
    public static class PaymentModuleExtensions
    {
        public static IServiceCollection AddPaymentModule(this IServiceCollection services)
        {
            services.AddScoped<IPaymentGateway, StripeGateway>();
            //services.AddScoped<IPaymentGateway, PaymobGateway>();
            services.AddScoped<IPaymentGatewayFactory, PaymentGatewayFactory>();
            services.AddScoped<IPaymentService, PaymentService>();

            return services;
        }
    }
}
