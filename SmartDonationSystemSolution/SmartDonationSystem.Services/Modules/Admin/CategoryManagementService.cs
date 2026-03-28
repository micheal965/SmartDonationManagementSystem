using Microsoft.EntityFrameworkCore;
using SmartDonationSystem.Core.Common.Models;
using SmartDonationSystem.Core.Modules.Admin.CategoryManagement.DTOs;
using SmartDonationSystem.Core.Modules.Admin.CategoryManagement.Interfaces;
using SmartDonationSystem.DataAccess;
using SmartDonationSystem.Shared.Responses;

namespace SmartDonationSystem.Services.Modules.Admin
{
    public class CategoryManagementService : ICategoryManagementService
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public CategoryManagementService(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
        public async Task<Result<CategoryToReturnDto>> CreateCategoryAsync(string categoryName, string description)
        {
            bool exists = await _applicationDbContext.Categories.AnyAsync(c => c.Name.ToLower() == categoryName.ToLower());
            if (exists)
                return Result<CategoryToReturnDto>.BadRequest("Category already exists");

            var category = new Category { Name = categoryName.Trim(), Description = description.Trim() };
            await _applicationDbContext.Categories.AddAsync(category);
            await _applicationDbContext.SaveChangesAsync();
            var categoryDto = new CategoryToReturnDto()
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                TotalPosts = 0
            };
            return Result<CategoryToReturnDto>.Created(categoryDto, "Category created successfully");
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
            return Result<object>.Ok(category, "Category deleted successfully");
        }

        public async Task<Result<List<CategoryToReturnDto>>> GetAllCategoriesAsync()
            => Result<List<CategoryToReturnDto>>.Ok(await _applicationDbContext.Categories
                                    .Select(c => new CategoryToReturnDto { Id = c.Id, Name = c.Name, Description = c.Description, TotalPosts = c.Posts.Count() })
                                    .ToListAsync());

        public async Task<Result<object>> UpdateCategoryAsync(int oldCategoryId, string newCategoryName, string newDescription)
        {
            Category? category = await _applicationDbContext.Categories.FindAsync(oldCategoryId);
            if (category == null) return Result<object>.NotFound("Category not found");

            bool exists = await _applicationDbContext.Categories.AnyAsync(c => c.Name.ToLower() == newCategoryName.ToLower()
                                                                        && c.Id != oldCategoryId);
            if (exists)
                return Result<object>.BadRequest("Another category with this name already exists.");

            if (!string.IsNullOrWhiteSpace(newCategoryName))
                category.Name = newCategoryName.Trim();

            if (!string.IsNullOrWhiteSpace(newDescription))
                category.Description = newDescription.Trim();

            await _applicationDbContext.SaveChangesAsync();
            return Result<object>.Ok(null, "Category updated successfully");
        }
    }
}
