using SmartDonationSystem.Core.Modules.Admin.CategoryManagement.Interfaces;
using SmartDonationSystem.Core.Modules.Admin.PostManagement.Interfaces;
using SmartDonationSystem.Core.Modules.Admin.PostManagement.MapsterConfigurations;
using SmartDonationSystem.Services.Modules.Admin.CategoryManagement;
using SmartDonationSystem.Services.Modules.Admin.PostManagement;

namespace SmartDonationSystem.API.Modules.Admin
{
    public static class AdminModuleExtensions
    {
        public static IServiceCollection AddAdminModule(this IServiceCollection services)
        {
            services.AddScoped<ICategoryManagementService, CategoryManagementService>();
            services.AddScoped<IPostManagementService, PostManagementService>();
            PostConfigs.PostMappings();
            return services;
        }
    }
}
