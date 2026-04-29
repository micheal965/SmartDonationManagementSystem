using SmartDonationSystem.Core.Modules.Notifications.Interfaces;
using SmartDonationSystem.Services.Modules.Notifications;

namespace SmartDonationSystem.API.Modules.User
{
    public static class PaymentModuleExtensions
    {
        public static IServiceCollection AddNotificationModule(this IServiceCollection services)
        {
            services.AddScoped<INotificationService, NotificationService>();
            return services;
        }
    }
}
