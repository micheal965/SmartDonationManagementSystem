using Microsoft.EntityFrameworkCore;
using SmartDonationSystem.Core.Modules.Admin.DashboardManagement.DTOs;
using SmartDonationSystem.Core.Modules.Admin.DashboardManagement.Interfaces;
using SmartDonationSystem.DataAccess;
using SmartDonationSystem.Shared.Responses;

namespace SmartDonationSystem.Services.Modules.Admin
{
    public class DashboardManagementService(ApplicationDbContext _context) : IDashboardManagementService
    {
        public async Task<Result<List<AnalyticsDto>>> GetLast30DaysAsync()
        {
            var analytics = await _context.AnalyticsEvents
                .Where(x => x.CreatedAt >= DateTime.UtcNow.AddDays(-30))
                .GroupBy(x => x.CreatedAt.Date)
                .Select(g => new AnalyticsDto
                {
                    Date = g.Key,
                    Count = g.Count()
                }).OrderBy(x => x.Date)
                .ToListAsync();
            return Result<List<AnalyticsDto>>.Ok(analytics);
        }
    }
}
