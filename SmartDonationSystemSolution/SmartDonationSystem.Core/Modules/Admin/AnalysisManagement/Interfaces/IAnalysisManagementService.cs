using SmartDonationSystem.Core.Modules.Admin.AnalysisManagement.DTOs;
using SmartDonationSystem.Shared.Responses;

namespace SmartDonationSystem.Core.Modules.Admin.AnalysisManagement.Interfaces
{
    public interface IAnalysisManagementService
    {
        Task<Result<AnalysisToReturnDto>> GetAnalysisDataAsync(DateTime? fromDate = null, DateTime? toDate = null);
    }
}
