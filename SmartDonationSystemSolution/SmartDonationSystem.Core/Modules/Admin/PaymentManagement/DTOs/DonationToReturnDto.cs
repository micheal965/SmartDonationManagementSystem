namespace SmartDonationSystem.Core.Modules.Admin.PaymentManagement.DTOs
{
    public class DonationToReturnDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public string PaymentGateway { get; set; }
        public int? PostId { get; set; }
        public string? PostTitle { get; set; }
        public string DonorId { get; set; }
        public string DonorName { get; set; }
        public string DonorEmail { get; set; }
        public string DonorPhoneNumber { get; set; }
        public string RequesterPhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
