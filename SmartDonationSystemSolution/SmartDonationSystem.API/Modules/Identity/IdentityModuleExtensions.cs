using SmartDonationSystem.Core.Modules.Auth.Interfaces;
using SmartDonationSystem.Core.Modules.Auth.MapsterConfigurations;
using SmartDonationSystem.Core.Modules.User.Interfaces;
using SmartDonationSystem.Services.Modules.Identity;

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
