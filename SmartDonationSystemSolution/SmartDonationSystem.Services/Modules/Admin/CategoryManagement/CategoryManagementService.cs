using Microsoft.EntityFrameworkCore;
using SmartDonationSystem.Core.Common.Models;
using SmartDonationSystem.Core.Modules.Admin.CategoryManagement.DTOs;
using SmartDonationSystem.Core.Modules.Admin.CategoryManagement.Interfaces;
using SmartDonationSystem.DataAccess;
using SmartDonationSystem.Shared.Responses;

namespace SmartDonationSystem.Services.Modules.Admin.CategoryManagement
{
    public class CategoryManagementService : ICategoryManagementService
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public CategoryManagementService(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
        public async Task<Result<object>> CreateCategoryAsync(string categoryName)
        {
            bool exists = await _applicationDbContext.Categories.AnyAsync(c => c.Name.ToLower() == categoryName.ToLower());
            if (exists)
                return Result<object>.BadRequest("Category already exists");

            await _applicationDbContext.Categories.AddAsync(new Category { Name = categoryName.Trim() });
            await _applicationDbContext.SaveChangesAsync();
            return Result<object>.Created("Category created successfully");
        }

        public async Task<Result<object>> DeleteCategoryAsync(int categoryId)
        {
            var category = await _applicationDbContext.Categories.FindAsync(categoryId);
            if (category == null)
                return Result<object>.NotFound("Category not found");

            bool hasPosts = await _applicationDbContext.Posts.AnyAsync(p => p.CategoryId == categoryId);
            if (hasPosts)
                return Result<object>.BadRequest("Cannot delete category because it has posts");

            _applicationDbContext.Categories.Remove(category);
            await _applicationDbContext.SaveChangesAsync();
            return Result<object>.Ok("Category deleted successfully");
        }

        public async Task<Result<List<CategoryToReturnDto>>> GetAllCategoriesAsync()
            => Result<List<CategoryToReturnDto>>.Ok(await _applicationDbContext.Categories
                                    .Select(c => new CategoryToReturnDto { Id = c.Id, Name = c.Name }).ToListAsync());

        public async Task<Result<object>> UpdateCategoryAsync(int oldCategoryId, string newCategoryName)
        {
            Category? category = await _applicationDbContext.Categories.FindAsync(oldCategoryId);
            if (category == null) return Result<object>.NotFound("Category not found");

            bool exists = await _applicationDbContext.Categories.AnyAsync(c => c.Name.ToLower() == newCategoryName.ToLower() && c.Id != oldCategoryId);
            if (exists)
                return Result<object>.BadRequest("Another category with this name already exists.");

            category.Name = newCategoryName.Trim();
            await _applicationDbContext.SaveChangesAsync();
            return Result<object>.Ok("Category updated successfully");
        }
    }
}
