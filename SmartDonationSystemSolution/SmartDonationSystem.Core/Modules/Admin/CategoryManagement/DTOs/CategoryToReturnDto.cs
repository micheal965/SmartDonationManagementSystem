namespace SmartDonationSystem.Core.Modules.Admin.CategoryManagement.DTOs
{
    public class CategoryToReturnDto
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required int TotalPosts { get; set; }
    }
}
