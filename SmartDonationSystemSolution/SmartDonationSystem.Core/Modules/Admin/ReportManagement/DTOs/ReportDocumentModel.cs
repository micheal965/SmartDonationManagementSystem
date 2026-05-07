using System;
using System.Collections.Generic;

namespace SmartDonationSystem.Core.Modules.Admin.ReportManagement.DTOs
{
    public class ReportDocumentModel
    {
        public string Title { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public List<string> Headers { get; set; } = new();
        public List<List<string>> Rows { get; set; } = new();
        public Dictionary<string, string>? Summary { get; set; }
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
