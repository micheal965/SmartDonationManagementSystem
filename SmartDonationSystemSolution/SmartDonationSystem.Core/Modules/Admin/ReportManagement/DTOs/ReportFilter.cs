namespace SmartDonationSystem.Core.Modules.Admin.ReportManagement.DTOs
{
    public class ReportFilter
    {
        public string Field { get; set; } = string.Empty;
        public string Operator { get; set; } = "eq";   // eq, neq, gt, lt, contains
        public string Value { get; set; } = string.Empty;
    }
}
