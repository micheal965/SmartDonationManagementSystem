using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SmartDonationSystem.API.Extensions;
using SmartDonationSystem.API.Filters;
using SmartDonationSystem.API.Middlewares;
using SmartDonationSystem.Core.Common.Models;
using SmartDonationSystem.Core.Modules.Analytics;
using SmartDonationSystem.DataAccess;
using SmartDonationSystem.Services.Modules.AI.ClassificationScoringModule;
using SmartDonationSystem.Services.Modules.SignalR.Hubs;
using SmartDonationSystem.Shared.Enums;
using Stripe;
using System.Text;
using System.Threading.RateLimiting;

namespace SmartDonationSystem.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            // Add services to the container.
            builder.Services.AddRateLimiter(options =>
            {
                options.AddPolicy("TrackPagePolicy", context =>
                {
                    // Ensure RemoteIpAddress is not null
                    var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                    return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 3,
                        Window = TimeSpan.FromMinutes(1),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 0
                    });
                });
            });

            builder.Services.AddControllers();
            builder.Services.AddSignalR();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddHangfire(options =>
                options.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;
            }).AddEntityFrameworkStores<ApplicationDbContext>()
              .AddDefaultTokenProviders();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                    ValidAudience = builder.Configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"])),
                    ClockSkew = TimeSpan.Zero
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                            context.Token = accessToken;

                        return Task.CompletedTask;
                    }
                };
            });

            builder.Services.AddAuthorization();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("FrontendPolicy", policy =>
                {
                    policy.WithOrigins("http://localhost:4200", "https://smart-donation-management-system.vercel.app")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });
            // Register Modules dependencies
            builder.Services.AddModulesDependencies();
            var keysPath = Path.Combine(builder.Environment.ContentRootPath, "keys");

            builder.Services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(keysPath))
                .SetApplicationName("MyApp");

            StripeConfiguration.ApiKey = builder.Configuration["Payments:Stripe:SecretKey"];

            var app = builder.Build();
            app.UseRateLimiter();

            app.MapPost("/api/track-page", async (IAnalyticsService service, HttpContext httpContext) =>
            {
                var user = httpContext.User;
                if (user.Identity?.IsAuthenticated == true && user.IsInRole(AppRoles.Admin))
                    return Results.Ok(new { message = "Admin page not tracked" });

                await service.TrackPageViewAsync();

                return Results.Ok(new { message = "Page tracked successfully" });
            }).RequireRateLimiting("TrackPagePolicy");

            app.UseCors("FrontendPolicy");

            //app.UseHangfireDashboard("/hangfire", new DashboardOptions
            //{
            //    Authorization = new[] { new HangfireAuthorizationFilter() }
            //});

            #region Hangfire Dashboard with its use cases
            app.UseHangfireDashboard();


            var recurringJobManager = app.Services.GetRequiredService<IRecurringJobManager>();
            recurringJobManager.AddOrUpdate<PostClassifierService>(
                "classify-posts-job",
                job => job.RunClassificationJobForAllCategoriesAsync(),
                Cron.DayInterval(1)
            );
            #endregion
            await SeedingData.SeedDataAsync(app);
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseMiddleware<LogoutMiddleware>();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            //}
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapHub<NotificationHub>("/hubs/notifications");
            app.MapHub<ChatHub>("/hubs/chat");
            app.Run();
        }
    }
}