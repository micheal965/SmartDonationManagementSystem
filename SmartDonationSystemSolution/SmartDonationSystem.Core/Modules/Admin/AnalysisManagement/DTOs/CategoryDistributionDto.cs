namespace SmartDonationSystem.Core.Modules.Admin.AnalysisManagement.DTOs
{
    public class CategoryDistributionDto
    {
        public string CategoryName { get; set; }
        public decimal TotalAmount { get; set; }
        public int DonationCount { get; set; }
    }
}
