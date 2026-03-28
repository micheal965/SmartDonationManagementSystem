using SmartDonationSystem.Core.Modules.Admin.CategoryManagement.DTOs;
using SmartDonationSystem.Shared.Responses;

namespace SmartDonationSystem.Core.Modules.Admin.CategoryManagement.Interfaces
{
    public interface ICategoryManagementService
    {
        Task<Result<CategoryToReturnDto>> CreateCategoryAsync(string categoryName, string description);
        Task<Result<object>> DeleteCategoryAsync(int categoryId);
        Task<Result<List<CategoryToReturnDto>>> GetAllCategoriesAsync();
        Task<Result<object>> UpdateCategoryAsync(int oldCategoryId, string newCategoryName, string newDescription);

    }
}
