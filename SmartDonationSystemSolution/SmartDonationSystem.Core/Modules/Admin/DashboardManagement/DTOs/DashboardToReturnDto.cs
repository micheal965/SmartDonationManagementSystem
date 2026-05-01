using SmartDonationSystem.Core.Modules.Admin.AnalysisManagement.DTOs;

namespace SmartDonationSystem.Core.Modules.Admin.DashboardManagement.DTOs
{
    public sealed class DashboardToReturnDto
    {
        public int TotalUsers { get; set; }
        public int TotalCategories { get; set; }
        public int TotalPublishedPosts { get; set; }
        //public int TotalNewNotifications { get; set; }
        public List<AnalyticsDto> analytics { get; set; }
        public int TotalUniqueUsers { get; set; }
    }
}
