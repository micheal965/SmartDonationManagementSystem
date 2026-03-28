using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartDonationSystem.Core.Modules.Admin.CategoryManagement.DTOs;
using SmartDonationSystem.Core.Modules.Admin.CategoryManagement.Interfaces;
using SmartDonationSystem.Shared.Enums;

namespace SmartDonationSystem.API.Modules.Admin.Controllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = AppRoles.Admin)]
    public class CategoryManagementController : ControllerBase
    {
        private readonly ICategoryManagementService _categoryService;

        public CategoryManagementController(ICategoryManagementService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpPost("create-category")]
        public async Task<IActionResult> CreateCategory([FromQuery] string categoryName, [FromQuery] string description)
        {
            var result = await _categoryService.CreateCategoryAsync(categoryName, description);
            return StatusCode((int)result.statusCode, result);
        }
        [HttpGet("get-categories")]
        public async Task<IActionResult> GetCategories()
        {
            var result = await _categoryService.GetAllCategoriesAsync();
            return StatusCode((int)result.statusCode, result);
        }
        [HttpDelete("delete-category")]
        public async Task<IActionResult> DeleteCategory([FromQuery] int categoryId)
        {
            var result = await _categoryService.DeleteCategoryAsync(categoryId);
            return StatusCode((int)result.statusCode, result);
        }
        [HttpPatch("update-category")]
        public async Task<IActionResult> UpdateCategory(UpdateCategoryDto updateCategoryDto)
        {
            var result = await _categoryService.UpdateCategoryAsync(updateCategoryDto.oldCategoryId,
                                                                    updateCategoryDto.newCategoryName,
                                                                    updateCategoryDto.newDescription);
            return StatusCode((int)result.statusCode, result);
        }
    }
}