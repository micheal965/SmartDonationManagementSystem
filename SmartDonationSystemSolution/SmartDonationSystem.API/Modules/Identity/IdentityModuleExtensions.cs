using SmartDonationSystem.Core.Auth.Interfaces;
using SmartDonationSystem.Core.Auth.MapsterConfigurations;
using SmartDonationSystem.Core.User.Interfaces;
using SmartDonationSystem.Services.Identity;

namespace SmartDonationSystem.API.Modules.Identity
{
    public static class IdentityModuleExtensions
    {
        public static IServiceCollection AddIdentityModule(this IServiceCollection services)
        {
            RegisterConfigs.RegisterMappings();
            services.AddScoped<IAuthServices, AuthServices>();
            services.AddScoped<IUserServices, UserServices>();
            return services;
        }
    }
}
