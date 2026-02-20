namespace SmartDonationSystem.Core.Modules.Admin.CategoryManagement.DTOs
{
    public class UpdateCategoryDto
    {
        public required int oldCategoryId { get; set; }
        public required string newCategoryName { get; set; }
    }
}
