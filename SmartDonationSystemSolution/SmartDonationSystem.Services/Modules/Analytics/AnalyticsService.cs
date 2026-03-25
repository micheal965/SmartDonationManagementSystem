using SmartDonationSystem.Core.Common.Models;
using SmartDonationSystem.Core.Modules.Analytics;
using SmartDonationSystem.DataAccess;
using SmartDonationSystem.Shared.Enums;

namespace SmartDonationSystem.Services.Modules.Analytics
{
    public class AnalyticsService(ApplicationDbContext _applicationDbContext) : IAnalyticsService
    {
        public async Task TrackPageViewAsync()
        {
            var ev = new AnalyticsEvent
            {
                Type = AnalyticsEventType.PageView,
                CreatedAt = DateTime.UtcNow
            };

            await _applicationDbContext.AnalyticsEvents.AddAsync(ev);
            await _applicationDbContext.SaveChangesAsync();
        }
    }
}
