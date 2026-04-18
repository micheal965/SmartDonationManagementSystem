using Microsoft.EntityFrameworkCore;
using SmartDonationSystem.Core.Modules.Categories.DTOs;
using SmartDonationSystem.Core.Modules.Categories.Interfaces;
using SmartDonationSystem.DataAccess;
using SmartDonationSystem.Shared.Responses;

namespace SmartDonationSystem.Services.Modules.Categories
{
    public class CategoryServices(ApplicationDbContext _applicationDbContext) : ICategoryServices
    {

        public async Task<Result<List<CategoryDetailsDto>>> GetAllCategoriesAsync()
    => Result<List<CategoryDetailsDto>>.Ok(await _applicationDbContext.Categories
                            .Select(c => new CategoryDetailsDto { Id = c.Id, Name = c.Name, Description = c.Description })
                            .ToListAsync());
    }
}
