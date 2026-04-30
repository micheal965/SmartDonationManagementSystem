using SmartDonationSystem.Core.Modules.User.Sidebar.DTOs;
using SmartDonationSystem.Shared.Responses;

namespace SmartDonationSystem.Core.Modules.User.Sidebar.Interfaces
{
    public interface ISidebarService
    {
        Task<Result<SidebarDataDto>> GetSidebarDataAsync();
    }
}
