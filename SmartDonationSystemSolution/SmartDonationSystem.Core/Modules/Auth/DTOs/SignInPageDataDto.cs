namespace SmartDonationSystem.Core.Modules.Auth.DTOs
{
    public class SignInPageDataDto
    {
        public RecentDonationDto? RecentDonation { get; set; }
        public TodayMilestonesDto Milestones { get; set; } = new TodayMilestonesDto();
    }

    public class RecentDonationDto
    {
        public string DonorName { get; set; }
        public decimal Amount { get; set; }
        public string PostTitle { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class TodayMilestonesDto
    {
        public int MealsDelivered { get; set; }
        public int ClassroomsBuilt { get; set; }
        public int CleanWaterLiters { get; set; }
    }
}
