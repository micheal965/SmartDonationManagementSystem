using SmartDonationSystem.Core.Modules.Admin.AnalysisManagement.Interfaces;
using SmartDonationSystem.Core.Modules.Admin.CategoryManagement.Interfaces;
using SmartDonationSystem.Core.Modules.Admin.DashboardManagement.Interfaces;
using SmartDonationSystem.Core.Modules.Admin.PaymentManagement.Interfaces;
using SmartDonationSystem.Core.Modules.Admin.PostManagement.Interfaces;
using SmartDonationSystem.Core.Modules.Admin.PostManagement.MapsterConfigurations;
using SmartDonationSystem.Core.Modules.Admin.ReportManagement.Interfaces;
using SmartDonationSystem.Core.Modules.Admin.UserManagement.Interfaces;
using SmartDonationSystem.Services.Modules.Admin;
using SmartDonationSystem.Services.Modules.Admin.PaymentManagement;
using SmartDonationSystem.Services.Modules.Admin.Reports;
using SmartDonationSystem.Services.Modules.Admin.Reports.Builders;
using SmartDonationSystem.Services.Modules.Admin.Reports.PdfGeneration;
using SmartDonationSystem.Services.Modules.Reports.Interfaces;


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
            services.AddScoped<IReportService, ReportManagementService>();

            services.AddScoped<IReportBuilder, DonationReportBuilder>();
            services.AddScoped<IReportBuilder, PostReportBuilder>();
            services.AddScoped<IReportBuilder, UserReportBuilder>();
            services.AddScoped<QuestPdfGenerator>();


            PostConfigs.PostMappings();
            return services;
        }
    }
}
