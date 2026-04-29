namespace SmartDonationSystem.Core.Modules.Payment.DTOs
{
    public class CreateDonationDto
    {
        public decimal Amount { get; set; }
        public int? PostId { get; set; }
        public string Gateway { get; set; } // "Stripe" or "Paymob"
    }
}
