using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SmartDonationSystem.Shared.Enums;

namespace SmartDonationSystem.DataAccess.Seed;

public static class DefaultRolesSeed
{
    public static async Task SeedRolesAsync(this IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        foreach (var role in Enum.GetNames(typeof(UserRole)))
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}
