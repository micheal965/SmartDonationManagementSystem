using SmartDonationSystem.Core.Modules.Admin.DashboardManagement.DTOs;
using SmartDonationSystem.Shared.Responses;

namespace SmartDonationSystem.Core.Modules.Admin.DashboardManagement.Interfaces
{
    public interface IDashboardManagementService
    {
        Task<Result<DashboardToReturnDto>> GetDashboardData();
    }
}
