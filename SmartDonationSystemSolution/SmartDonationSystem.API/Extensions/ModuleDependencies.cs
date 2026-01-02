using Microsoft.AspNetCore.Mvc;
using SmartDonationSystem.API.Identity;
using SmartDonationSystem.Core.Cloud;
using SmartDonationSystem.Services.Cloud;
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

            services.AddIdentityModule();
            services.AddScoped<ICloudinaryServices, CloudinaryServices>();
            return services;
        }
    }
}
