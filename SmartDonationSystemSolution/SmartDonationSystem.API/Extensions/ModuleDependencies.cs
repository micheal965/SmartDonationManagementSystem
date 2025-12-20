using SmartDonationSystem.API.Controllers.Auth;

namespace SmartDonationSystem.API.Extensions
{
    public static class ModuleDependencies
    {
        public static IServiceCollection AddModulesDependencies(this IServiceCollection services)
        {
            services.AddAuthModule();
            return services;
        }
    }
}
