using SmartDonationSystem.Core.Modules.Admin.ReportManagement.Enums;

namespace SmartDonationSystem.Core.Modules.Admin.ReportManagement.DTOs
{
    public class ReportRequest
    {
        public ReportType ReportType { get; set; }
        public List<ReportFilter> Filters { get; set; } = new();
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }
}
