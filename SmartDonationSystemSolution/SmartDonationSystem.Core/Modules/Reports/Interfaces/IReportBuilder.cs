using System.Threading.Tasks;
using SmartDonationSystem.Core.Modules.Admin.ReportManagement.DTOs;
using SmartDonationSystem.Core.Modules.Admin.ReportManagement.Enums;

namespace SmartDonationSystem.Services.Modules.Reports.Interfaces
{
    public interface IReportBuilder
    {
        ReportType SupportedType { get; }
        Task<ReportDocumentModel> BuildAsync(ReportRequest request);
    }
}
