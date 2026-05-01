namespace SmartDonationSystem.Core.Modules.User.UserAnalysis.DTOs
{
    public class PlatformAnalysisDto
    {
        public decimal TotalDonationsProcessed { get; set; }
        public int TotalSuccessfulTransactions { get; set; }
        public int TotalCausesFulfilled { get; set; }
        public int TotalActiveCauses { get; set; }
        public int TotalDonors { get; set; }
        public int TotalRequesters { get; set; }
        public List<CategoryDistributionDto> TopCategories { get; set; } = new List<CategoryDistributionDto>();
        public List<TrendDto> PlatformGrowthTrend { get; set; } = new List<TrendDto>();
    }
}
