using SmartDonationSystem.Core.Modules.User.UserAnalysis.DTOs;
using SmartDonationSystem.Shared.Responses;

namespace SmartDonationSystem.Core.Modules.User.UserAnalysis.Interfaces
{
    public interface IUserAnalysisService
    {
        Task<Result<UserAnalysisDto>> GetUserAnalysisAsync();
        Task<Result<PlatformAnalysisDto>> GetPlatformAnalysisAsync();
    }
}
