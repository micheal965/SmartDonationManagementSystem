namespace SmartDonationSystem.Core.Modules.User.UserAnalysis.DTOs
{
    public class UserAnalysisDto
    {
        public DonorImpactDto DonorImpact { get; set; } = new DonorImpactDto();
        public RequesterImpactDto RequesterImpact { get; set; } = new RequesterImpactDto();
    }

    public class DonorImpactDto
    {
        public decimal TotalDonated { get; set; }
        public int TotalCausesSupported { get; set; }
        public List<CategoryDistributionDto> CategoriesSupported { get; set; } = new List<CategoryDistributionDto>();
        public List<TrendDto> DonationTrend { get; set; } = new List<TrendDto>();
    }

    public class RequesterImpactDto
    {
        public decimal TotalFundsRaised { get; set; }
        public int TotalNeedsFulfilled { get; set; }
        public int ActiveNeeds { get; set; }
        public List<TrendDto> FundsRaisedTrend { get; set; } = new List<TrendDto>();
    }

    public class CategoryDistributionDto
    {
        public string CategoryName { get; set; }
        public decimal TotalAmount { get; set; }
        public int DonationCount { get; set; }
    }

    public class TrendDto
    {
        public DateTime Date { get; set; }
        public decimal Value { get; set; }
    }
}
