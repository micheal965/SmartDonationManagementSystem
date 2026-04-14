using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartDonationSystem.Core.Common.Models;

namespace SmartDonationSystem.DataAccess.Configurations
{
    public class ConversationConfigurations : IEntityTypeConfiguration<Conversation>
    {
        public void Configure(EntityTypeBuilder<Conversation> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.User1Id).IsRequired();
            builder.Property(c => c.User2Id).IsRequired();

            builder.HasIndex(c => new { c.User1Id, c.User2Id }).IsUnique();
        }
    }
}
