using Microsoft.AspNetCore.Identity;
using SmartDonationSystem.Core.Auths;
using SmartDonationSystem.Shared.Enums;

namespace SmartDonationSystem.DataAccess.Seed;

public static class DefaultUsersSeed
{
    public static async Task SeedAdminAsync(this UserManager<ApplicationUser> userManager)
    {
        var user = new ApplicationUser()
        {
            IdentityNumber = "30407142102037",
            Email = "michealghobriall@gmail.com",
            UserName = "MichealGhobrial",
            PhoneNumber = "01201605049",
            EmailConfirmed = true,
        };
        await userManager.CreateAsync(user, "P@$$w0rd");
        await userManager.AddToRoleAsync(user, UserRole.Admin.ToString());
    }
}
