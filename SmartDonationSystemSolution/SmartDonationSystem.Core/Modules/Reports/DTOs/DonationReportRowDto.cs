namespace SmartDonationSystem.Services.Modules.Reports.DTOs
{
    public class DonationReportRowDto
    {
        public string DonorName { get; set; } = string.Empty;
        public string RequesterName { get; set; } = string.Empty;
        public string PostTitle { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PaymentGateway { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
