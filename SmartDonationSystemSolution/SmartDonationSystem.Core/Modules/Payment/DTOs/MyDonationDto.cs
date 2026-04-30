namespace SmartDonationSystem.Core.Modules.Payment.DTOs
{
    public class MyDonationDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public string? CheckoutUrl { get; set; }
        public string PaymentGateway { get; set; }
        public int? PostId { get; set; }
        public string? PostTitle { get; set; }
        public string? PostPicture { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
