using Microsoft.AspNetCore.Mvc;
using SmartDonationSystem.Core.Modules.Categories.Interfaces;

namespace SmartDonationSystem.API.Modules.User.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryServices _categoryService;
        public CategoryController(ICategoryServices categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("get-categories")]
        public async Task<IActionResult> GetCategories()
        {
            var result = await _categoryService.GetAllCategoriesAsync();
            return StatusCode((int)result.statusCode, result);
        }
    }
}
