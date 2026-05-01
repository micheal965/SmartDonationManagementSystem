using System.Collections.Generic;

namespace SmartDonationSystem.Core.Modules.Admin.AnalysisManagement.DTOs
{
    public class CategoryTrendDto
    {
        public string CategoryName { get; set; }
        public List<TrendDto> Trends { get; set; }
    }
}
