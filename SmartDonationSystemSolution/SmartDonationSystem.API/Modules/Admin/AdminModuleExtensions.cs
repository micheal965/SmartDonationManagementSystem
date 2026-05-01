using SmartDonationSystem.Core.Modules.Admin.AnalysisManagement.Interfaces;
using SmartDonationSystem.Core.Modules.Admin.CategoryManagement.Interfaces;
using SmartDonationSystem.Core.Modules.Admin.DashboardManagement.Interfaces;
using SmartDonationSystem.Core.Modules.Admin.PaymentManagement.Interfaces;
using SmartDonationSystem.Core.Modules.Admin.PostManagement.Interfaces;
using SmartDonationSystem.Core.Modules.Admin.PostManagement.MapsterConfigurations;
using SmartDonationSystem.Core.Modules.Admin.UserManagement.Interfaces;
using SmartDonationSystem.Services.Modules.Admin;
using SmartDonationSystem.Services.Modules.Admin.PaymentManagement;

namespace SmartDonationSystem.API.Modules.Admin
{
    public static class AdminModuleExtensions
    {
        public static IServiceCollection AddAdminModule(this IServiceCollection services)
        {
            services.AddScoped<IDashboardManagementService, DashboardManagementService>();
            services.AddScoped<IUserManagementService, UserManagementService>();
            services.AddScoped<ICategoryManagementService, CategoryManagementService>();
            services.AddScoped<IPostManagementService, PostManagementService>();
            services.AddScoped<IPaymentManagementService, PaymentManagementService>();
            services.AddScoped<IAnalysisManagementService, AnalysisManagementService>();

            PostConfigs.PostMappings();
            return services;
        }
    }
}
