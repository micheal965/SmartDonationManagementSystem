using SmartDonationSystem.Shared.Enums;

namespace SmartDonationSystem.Core.Common.Models
{
    public class Donation : BaseEntity
    {
        public decimal Amount { get; set; }
        public DonationStatus Status { get; set; } = DonationStatus.Pending;

        public DonationType Type { get; set; } // "Post" , "Platform"

        public string PaymentGateway { get; set; } // "Stripe", "Paymob"
        public string? PaymentGatewayId { get; set; }

        public int? PostId { get; set; }
        public Post? Post { get; set; }

        public required string DonorId { get; set; }
        public ApplicationUser Donor { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
