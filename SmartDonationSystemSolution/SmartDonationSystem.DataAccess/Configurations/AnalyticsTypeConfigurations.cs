using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartDonationSystem.Core.Common.Models;

namespace SmartDonationSystem.DataAccess.Configurations
{
    public class AnalyticsTypeConfigurations : IEntityTypeConfiguration<AnalyticsEvent>
    {
        public void Configure(EntityTypeBuilder<AnalyticsEvent> builder)
        {
            builder.HasIndex(x => x.CreatedAt);

            builder.HasIndex(x => x.Type);

            builder.HasIndex(x => x.PostId);

            builder.HasOne(a => a.Post)
                .WithMany(p => p.AnalyticsEvents)
                .HasForeignKey(a => a.PostId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(x => new { x.PostId, x.ApplicationUserId })
                .IsUnique();
        }
    }
}
