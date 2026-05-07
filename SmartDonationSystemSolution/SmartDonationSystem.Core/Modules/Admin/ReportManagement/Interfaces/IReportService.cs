using System.Threading.Tasks;
using SmartDonationSystem.Core.Modules.Admin.ReportManagement.DTOs;
using SmartDonationSystem.Shared.Responses;

namespace SmartDonationSystem.Core.Modules.Admin.ReportManagement.Interfaces
{
    public interface IReportService
    {
        Task<Result<byte[]>> GeneratePdfAsync(ReportRequest request, string? logoPath = null);
    }
}
