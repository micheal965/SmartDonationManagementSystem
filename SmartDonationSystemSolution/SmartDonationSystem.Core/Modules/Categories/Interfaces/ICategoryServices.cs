using SmartDonationSystem.Core.Modules.Categories.DTOs;
using SmartDonationSystem.Shared.Responses;

namespace SmartDonationSystem.Core.Modules.Categories.Interfaces
{
    public interface ICategoryServices
    {
        Task<Result<List<CategoryDetailsDto>>> GetAllCategoriesAsync();
    }
}
