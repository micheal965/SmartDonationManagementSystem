namespace SmartDonationSystem.Core.Modules.Admin.AnalysisManagement.DTOs
{
    public class AnalysisToReturnDto
    {
        public decimal TotalDonationAmount { get; set; }
        public int TotalPaidAndProcessedDonations { get; set; }
        public int TotalPaidDonations { get; set; }
        public int TotalProcessedToClientDonations { get; set; }
        public int TotalCompletedTargets { get; set; }
        public int TotalDonors { get; set; }
        public List<TrendDto> DonationTrend { get; set; }
        public List<CategoryTrendDto> CategoryTrends { get; set; }
        public List<CategoryDistributionDto> CategoryBreakdown { get; set; }
        public List<StatusDistributionDto> StatusBreakdown { get; set; }
    }
}
