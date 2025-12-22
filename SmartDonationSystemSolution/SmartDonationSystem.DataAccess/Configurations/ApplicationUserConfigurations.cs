using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartDonationSystem.Core.Auth.Models;

namespace SmartDonationSystem.DataAccess.Configurations;

public class ApplicationUserConfigurations : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasIndex(u => u.IdentityNumber).IsUnique();
        builder.Property(u => u.IdentityNumber).HasMaxLength(14).IsRequired();
        builder.Property(u => u.PictureUrl).HasMaxLength(250);
        builder.Ignore(u => u.UserName);

        // 1 ApplicationUser has many loginHistory
        builder.HasMany(u => u.UserLoginsHistory)
               .WithOne(lg => lg.ApplicationUser)
               .HasForeignKey(lg => lg.ApplicationUserId)
               .OnDelete(DeleteBehavior.Cascade);

        // 1 ApplicationUser has many refreshToken
        builder.HasMany(u => u.RefreshTokens)
               .WithOne(rf => rf.ApplicationUser)
               .HasForeignKey(rf => rf.ApplicationUserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
