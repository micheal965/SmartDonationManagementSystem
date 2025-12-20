using SmartDonationSystem.Core.Auth.Interfaces;
using SmartDonationSystem.Services.Auth;

namespace SmartDonationSystem.API.Identity
{
    public static class AuthModuleExtensions
    {
        public static IServiceCollection AddAuthModule(this IServiceCollection services)
        {
            services.AddScoped<IAuthOcrService, AuthOcrService>();
            services.AddScoped<IAuthServices, AuthServices>();
            return services;
        }
    }
}
