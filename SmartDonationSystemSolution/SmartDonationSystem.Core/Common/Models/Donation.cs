using SmartDonationSystem.Shared.Enums;

namespace SmartDonationSystem.Core.Common.Models
{
    public class Donation : BaseEntity
    {
        public decimal Amount { get; set; }
        public string Status { get; set; } = DonationStatus.Pending.ToString();
        public string? CheckoutUrl { get; set; }

        public string Type { get; set; } // "Post" , "Platform"

        public string PaymentGateway { get; set; } // "Stripe", "Paymob"

        public int? PostId { get; set; }
        public Post? Post { get; set; }

        public required string DonorId { get; set; }
        public ApplicationUser Donor { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
