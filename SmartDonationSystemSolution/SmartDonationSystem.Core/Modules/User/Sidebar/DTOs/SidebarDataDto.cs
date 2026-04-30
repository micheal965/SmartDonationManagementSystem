namespace SmartDonationSystem.Core.Modules.User.Sidebar.DTOs
{
    public class SidebarDataDto
    {
        public List<LiveImpactDto> LiveImpacts { get; set; } = new List<LiveImpactDto>();
        public List<TrendingNeedDto> TrendingNeeds { get; set; } = new List<TrendingNeedDto>();
        public TotalImpactDto TotalImpact { get; set; } = new TotalImpactDto();
    }

    public class LiveImpactDto
    {
        public string DonorName { get; set; }
        public string DonorPicture { get; set; }
        public decimal Amount { get; set; }
        public string PostTitle { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class TrendingNeedDto
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string CategoryName { get; set; }
        public int? PriorityLevel { get; set; }
    }

    public class TotalImpactDto
    {
        public decimal TotalAmountToday { get; set; }
        public int VerifiedCasesCount { get; set; }
    }
}
