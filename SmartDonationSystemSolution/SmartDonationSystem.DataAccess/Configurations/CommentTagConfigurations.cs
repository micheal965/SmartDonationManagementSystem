using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartDonationSystem.Core.Common.Models;

namespace SmartDonationSystem.DataAccess.Configurations
{
    public class CommentTagConfigurations : IEntityTypeConfiguration<CommentTag>
    {
        public void Configure(EntityTypeBuilder<CommentTag> builder)
        {
            builder.HasOne(ct => ct.Comment)
                .WithMany(c => c.Mentions)
                .HasForeignKey(cm => cm.CommentId);
        }
    }
}
