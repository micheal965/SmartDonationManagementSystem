namespace SmartDonationSystem.Services.Modules.Reports.DTOs
{
    public class PostReportRowDto
    {
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal TargetMoney { get; set; }
        public decimal CollectedMoney { get; set; }
        public string Status { get; set; } = string.Empty;
        public string CreatedByRole { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
