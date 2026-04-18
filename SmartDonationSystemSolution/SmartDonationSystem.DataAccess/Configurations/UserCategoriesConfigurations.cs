using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartDonationSystem.Core.Common.Models;

namespace SmartDonationSystem.DataAccess.Configurations
{
    public class UserCategoriesConfigurations : IEntityTypeConfiguration<UserCategory>
    {
        public void Configure(EntityTypeBuilder<UserCategory> builder)
        {
            builder.HasKey(uc => new { uc.UserId, uc.CategoryId });

            builder.HasOne(uc => uc.User)
            .WithMany(u => u.UserCategories)
            .HasForeignKey(uc => uc.UserId);

            builder.HasOne(uc => uc.Category)
                 .WithMany(c => c.UserCategories)
                 .HasForeignKey(uc => uc.CategoryId);
        }
    }
}
