using System.ComponentModel.DataAnnotations;

namespace SmartDonationSystem.Core.Modules.Admin.CategoryManagement.DTOs
{
    public class UpdateCategoryDto
    {
        [Required]
        public required int oldCategoryId { get; set; }
        public string newCategoryName { get; set; }
        public string newDescription { get; set; }
    }
}
