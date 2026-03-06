using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartDonationSystem.Core.Common.Models;

namespace SmartDonationSystem.DataAccess.Configurations;

public class ApplicationUserConfigurations : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasIndex(u => u.IdentityNumber).IsUnique();
        builder.Property(u => u.IdentityNumber).HasMaxLength(14).IsRequired();
        builder.Property(u => u.PictureUrl).HasMaxLength(250);

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

        // 1 ApplicationUser has many posts
        builder.HasMany(u => u.Posts)
                .WithOne(p => p.ApplicationUser)
                .HasForeignKey(p => p.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

        // 1 ApplicationUser has many reactions
        builder.HasMany(u => u.Reactions)
                .WithOne(r => r.ApplicationUser)
                .HasForeignKey(r => r.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);

        // 1 ApplicationUser has many Comments
        builder.HasMany(u => u.Comments)
                .WithOne(r => r.ApplicationUser)
                .HasForeignKey(r => r.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);

        // 1 ApplicationUser has many CommentTags
        builder.HasMany(u => u.CommentTags)
                .WithOne(r => r.MentionedUser)
                .HasForeignKey(r => r.MentionedUserId)
                .OnDelete(DeleteBehavior.Restrict);
    }
}
