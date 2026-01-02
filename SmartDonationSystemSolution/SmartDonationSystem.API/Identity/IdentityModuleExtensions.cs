using SmartDonationSystem.Core.Auth.Interfaces;
using SmartDonationSystem.Services.Identity;

namespace SmartDonationSystem.API.Identity
{
    public static class IdentityModuleExtensions
    {
        public static IServiceCollection AddIdentityModule(this IServiceCollection services)
        {
            services.AddScoped<IAuthOcrService, AuthOcrService>();
            services.AddScoped<IAuthServices, AuthServices>();
            return services;
        }
    }
}
