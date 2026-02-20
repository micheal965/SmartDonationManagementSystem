using SmartDonationSystem.Core.Modules.Auth.Interfaces;
using SmartDonationSystem.Core.Modules.Auth.MapsterConfigurations;
using SmartDonationSystem.Services.Modules.Identity;

namespace SmartDonationSystem.API.Modules.Identity
{
    public static class IdentityModuleExtensions
    {
        public static IServiceCollection AddIdentityModule(this IServiceCollection services)
        {
            RegisterConfigs.RegisterMappings();
            services.AddScoped<IAuthService, AuthServices>();
            return services;
        }
    }
}
