using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartDonationSystem.Core.Common.Models;

namespace SmartDonationSystem.DataAccess.Configurations
{
    public class NotificationConfigurations : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.Property(x => x.Title).HasMaxLength(200);
            builder.Property(x => x.Message).HasMaxLength(1000);

            builder.Property(x => x.Type)
                  .HasConversion<string>();

            builder.HasIndex(x => new { x.ReceiverId, x.IsRead });
            builder.HasIndex(x => x.CreatedAt);
        }
    }
}
