using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartDonationSystem.Core.Common.Models;

namespace SmartDonationSystem.DataAccess.Configurations
{
    public class DonationConfigurations : IEntityTypeConfiguration<Donation>
    {
        public void Configure(EntityTypeBuilder<Donation> builder)
        {
            builder.Property(d => d.Amount)
                .HasColumnType("decimal(18,2)");

            builder.HasOne(d => d.Post)
                .WithMany(p => p.Donations)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(d => d.Donor)
                .WithMany()
                .HasForeignKey(d => d.DonorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
