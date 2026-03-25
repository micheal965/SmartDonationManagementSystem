using SmartDonationSystem.Core.Modules.Admin.CategoryManagement.Interfaces;
using SmartDonationSystem.Core.Modules.Admin.DashboardManagement.Interfaces;
using SmartDonationSystem.Core.Modules.Admin.PostManagement.Interfaces;
using SmartDonationSystem.Core.Modules.Admin.PostManagement.MapsterConfigurations;
using SmartDonationSystem.Services.Modules.Admin;

namespace SmartDonationSystem.API.Modules.Admin
{
    public static class AdminModuleExtensions
    {
        public static IServiceCollection AddAdminModule(this IServiceCollection services)
        {
            services.AddScoped<ICategoryManagementService, CategoryManagementService>();
            services.AddScoped<IPostManagementService, PostManagementService>();
            services.AddScoped<IDashboardManagementService, DashboardManagementService>();
            PostConfigs.PostMappings();
            return services;
        }
    }
}
