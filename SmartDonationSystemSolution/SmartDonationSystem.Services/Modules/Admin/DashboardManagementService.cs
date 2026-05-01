using Microsoft.EntityFrameworkCore;
using SmartDonationSystem.Core.Modules.Admin.AnalysisManagement.DTOs;
using SmartDonationSystem.Core.Modules.Admin.DashboardManagement.DTOs;
using SmartDonationSystem.Core.Modules.Admin.DashboardManagement.Interfaces;
using SmartDonationSystem.DataAccess;
using SmartDonationSystem.Shared.Enums;
using SmartDonationSystem.Shared.Responses;

namespace SmartDonationSystem.Services.Modules.Admin
{
    public class DashboardManagementService(ApplicationDbContext _context) : IDashboardManagementService
    {
        public async Task<Result<DashboardToReturnDto>> GetDashboardData()
        {
            var analytics = await GetLast30DaysAsync();
            var totalUsers = await _context.Users.CountAsync();
            var totalCategories = await _context.Categories.CountAsync();
            var totalPosts = await _context.Posts.Where(p => p.Status == PostStatus.Approved.ToString()).CountAsync();

            // Fill missing dates
            var analyticsFull = Enumerable.Range(0, 30)
                .Select(i => DateTime.UtcNow.Date.AddDays(-29 + i))
                .Select(date => analytics.FirstOrDefault(a => a.Date.Date == date) ?? new AnalyticsDto { Date = date, Count = 0 })
                .ToList();

            var dashboardDto = new DashboardToReturnDto
            {
                analytics = analyticsFull,
                TotalUsers = totalUsers,
                TotalCategories = totalCategories,
                TotalPublishedPosts = totalPosts,
                TotalUniqueUsers = analytics.Sum(x => x.Count),
            };

            return Result<DashboardToReturnDto>.Ok(dashboardDto);
        }
        private async Task<List<AnalyticsDto>> GetLast30DaysAsync()
            => await _context.AnalyticsEvents
                .Where(x => x.CreatedAt >= DateTime.UtcNow.AddDays(-30))
                .GroupBy(x => x.CreatedAt.Date)
                .Select(g => new AnalyticsDto
                {
                    Date = g.Key,
                    Count = g.Count()
                }).OrderBy(x => x.Date)
                .ToListAsync();
    }
}
