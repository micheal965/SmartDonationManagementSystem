using Hangfire;
using Microsoft.AspNetCore.Mvc;
using SmartDonationSystem.API.Modules.Admin;
using SmartDonationSystem.API.Modules.Identity;
using SmartDonationSystem.API.Modules.Messaging;
using SmartDonationSystem.API.Modules.User;
using SmartDonationSystem.Core.Modules.AI;
using SmartDonationSystem.Core.Modules.Analytics;
using SmartDonationSystem.Core.Modules.Categories.Interfaces;
using SmartDonationSystem.Core.Modules.Cloud;
using SmartDonationSystem.Core.Modules.Encryption.Interfaces;
using SmartDonationSystem.Core.Modules.FileExtractionModule;
using SmartDonationSystem.Services.Modules.AI;
using SmartDonationSystem.Services.Modules.AI.ClassificationScoringModule;
using SmartDonationSystem.Services.Modules.AI.SummarizationModule;
using SmartDonationSystem.Services.Modules.Analytics;
using SmartDonationSystem.Services.Modules.Categories;
using SmartDonationSystem.Services.Modules.Cloud;
using SmartDonationSystem.Services.Modules.Encryption;
using SmartDonationSystem.Services.Modules.FileExtractionModule;
using SmartDonationSystem.Shared.Responses;

namespace SmartDonationSystem.API.Extensions
{
    public static class ModuleDependencies
    {
        public static IServiceCollection AddModulesDependencies(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(ms => ms.Value.Errors.Any())
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value.Errors
                                .Select(e => e.ErrorMessage)
                                .ToArray()
                        );

                    var result = Result<object>.BadRequest(
                        "Validation failed",
                        errors
                    );
                    return new BadRequestObjectResult(result);
                };
            });

            services.AddHangfireServer();

            services.AddIdentityModule();
            services.AddUserModule();
            services.AddAdminModule();
            services.AddNotificationModule();
            services.AddMessagingModule();

            services.AddDataProtection();
            services.AddScoped<ICategoryServices, CategoryServices>();
            services.AddScoped<ICloudinaryServices, CloudinaryServices>();
            services.AddScoped<IAnalyticsService, AnalyticsService>();
            services.AddScoped<IEncryptionService, EncryptionService>();

            #region Summarization Module
            services.AddScoped<ChunkingService>();
            services.AddScoped<PromptBuilder>();
            services.AddScoped<SummarizationService>();
            services.AddScoped<SummaryJob>();
            #endregion
            #region Classification service
            services.AddScoped<PostClassifierService>();


            #endregion
            #region Gemini AI Client
            services.AddHttpClient<GeminiClient>();
            services.AddScoped<IAIClient, GeminiClient>();
            #endregion
            #region File Extraction
            services.AddScoped<PDFExtractor>();
            services.AddScoped<TxtExtractor>();
            services.AddScoped<DocxExtractor>();
            services.AddScoped<IFileExtractionService>(sp => sp.GetRequiredService<PDFExtractor>());
            services.AddScoped<IFileExtractionService>(sp => sp.GetRequiredService<TxtExtractor>());
            services.AddScoped<IFileExtractionService>(sp => sp.GetRequiredService<DocxExtractor>());
            services.AddScoped<IEnumerable<IFileExtractionService>>(sp => new IFileExtractionService[]
            {
                sp.GetRequiredService<PDFExtractor>(),
                sp.GetRequiredService<TxtExtractor>(),
                sp.GetRequiredService<DocxExtractor>()
            });
            services.AddScoped<FileExtractionJob>();
            services.AddScoped<FileExtractionOrchestrator>();
            #endregion
            return services;
        }
    }
}
