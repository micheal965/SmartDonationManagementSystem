using System;

namespace SmartDonationSystem.Services.Modules.Reports.DTOs
{
    public class UserReportRowDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
