using Microsoft.AspNetCore.Identity;
using SmartDonationSystem.Core.Auth.Models;
using SmartDonationSystem.DataAccess.Seed;

namespace SmartDonationSystem.API.Extensions;

public static class SeedingData
{
    public static async Task SeedDataAsync(WebApplication app)
    {
        using var scope = app.Services.CreateAsyncScope();
        var services = scope.ServiceProvider;

        //Seeding Roles
        await services.SeedRolesAsync();
        //Seeding Users
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        await userManager.SeedAdminAsync();
    }
}
