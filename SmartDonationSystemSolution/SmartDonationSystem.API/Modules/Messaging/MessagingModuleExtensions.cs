using SmartDonationSystem.Core.Modules.Messaging.Interfaces;
using SmartDonationSystem.Services.Modules.Messaging;

namespace SmartDonationSystem.API.Modules.Messaging
{
    public static class MessagingModuleExtensions
    {
        public static IServiceCollection AddMessagingModule(this IServiceCollection services)
        {
            services.AddScoped<IChatService, ChatService>();
            return services;
        }
    }
}
