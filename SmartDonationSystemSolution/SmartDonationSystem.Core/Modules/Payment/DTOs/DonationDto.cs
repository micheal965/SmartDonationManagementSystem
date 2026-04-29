using SmartDonationSystem.Shared.Enums;

namespace SmartDonationSystem.Core.Modules.Payment.DTOs
{
    public class DonationDto
    {
        public int Id { get; set; }

        public decimal Amount { get; set; }

        public DonationStatus Status { get; set; }

        public DonationType Type { get; set; }

        public string PaymentGateway { get; set; }

        public int? PostId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
