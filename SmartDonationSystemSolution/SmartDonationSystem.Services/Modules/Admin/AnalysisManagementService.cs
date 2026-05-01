using Microsoft.EntityFrameworkCore;
using SmartDonationSystem.Core.Modules.Admin.AnalysisManagement.DTOs;
using SmartDonationSystem.Core.Modules.Admin.AnalysisManagement.Interfaces;
using SmartDonationSystem.DataAccess;
using SmartDonationSystem.Shared.Enums;
using SmartDonationSystem.Shared.Responses;

namespace SmartDonationSystem.Services.Modules.Admin
{
    public class AnalysisManagementService : IAnalysisManagementService
    {
        private readonly ApplicationDbContext _context;

        public AnalysisManagementService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Result<AnalysisToReturnDto>> GetAnalysisDataAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var successfulDonationStatuses = new[] { DonationStatus.Paid.ToString(), DonationStatus.Processed.ToString() };

            var startDate = fromDate ?? DateTime.UtcNow.AddDays(-30).Date;
            var endDate = toDate ?? DateTime.UtcNow.Date;
            var daysCount = (int)(endDate - startDate).TotalDays + 1;

            var totalPaidAndProcessedDonations = await _context.Donations
                .CountAsync(d => successfulDonationStatuses.Contains(d.Status) && d.CreatedAt.Date >= startDate && d.CreatedAt.Date <= endDate);

            var totalPaidDonations = await _context.Donations
                .CountAsync(d => d.Status == DonationStatus.Paid.ToString() && d.CreatedAt.Date >= startDate && d.CreatedAt.Date <= endDate);

            var totalProcessedToClientDonations = await _context.Donations
                .CountAsync(d => d.Status == DonationStatus.Processed.ToString() && d.CreatedAt.Date >= startDate && d.CreatedAt.Date <= endDate);

            var totalDonationAmount = await _context.Donations
                .Where(d => successfulDonationStatuses.Contains(d.Status) && d.CreatedAt.Date >= startDate && d.CreatedAt.Date <= endDate)
                .SumAsync(d => d.Amount);

            var totalDonors = await _context.Donations
                .Where(d => successfulDonationStatuses.Contains(d.Status) && d.CreatedAt.Date >= startDate && d.CreatedAt.Date <= endDate)
                .Select(d => d.DonorId)
                .Distinct()
                .CountAsync();

            var totalCompletedTargets = await _context.Posts
                .Where(p => p.TargetMoney != null && p.TargetMoney > 0)
                .Select(p => new
                {
                    p.TargetMoney,
                    CurrentDonations = p.Donations!.Where(d => successfulDonationStatuses.Contains(d.Status) && d.CreatedAt.Date >= startDate && d.CreatedAt.Date <= endDate).Sum(d => d.Amount)
                })
                .Where(x => x.CurrentDonations >= x.TargetMoney)
                .CountAsync();

            var donationTrend = await _context.Donations
                .Where(d => successfulDonationStatuses.Contains(d.Status) && d.CreatedAt.Date >= startDate && d.CreatedAt.Date <= endDate)
                .GroupBy(d => d.CreatedAt.Date)
                .Select(g => new TrendDto
                {
                    Date = g.Key,
                    Value = g.Sum(d => d.Amount)
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            // Fill missing dates for trend
            var fullTrend = Enumerable.Range(0, daysCount)
                .Select(i => startDate.AddDays(i))
                .Select(date => donationTrend.FirstOrDefault(t => t.Date.Date == date) ?? new TrendDto { Date = date, Value = 0 })
                .ToList();

            var categoryBreakdown = await _context.Donations
                .Where(d => successfulDonationStatuses.Contains(d.Status) && d.PostId != null && d.CreatedAt.Date >= startDate && d.CreatedAt.Date <= endDate)
                .GroupBy(d => d.Post!.Category.Name)
                .Select(g => new CategoryDistributionDto
                {
                    CategoryName = g.Key,
                    TotalAmount = g.Sum(d => d.Amount),
                    DonationCount = g.Count()
                })
                .ToListAsync();

            var statusBreakdown = await _context.Donations
                .Where(d => d.CreatedAt.Date >= startDate && d.CreatedAt.Date <= endDate)
                .GroupBy(d => d.Status)
                .Select(g => new StatusDistributionDto
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            // Category Trends
            var categoryTrendsRaw = await _context.Donations
                .Where(d => successfulDonationStatuses.Contains(d.Status) && d.CreatedAt.Date >= startDate && d.CreatedAt.Date <= endDate && d.PostId != null)
                .GroupBy(d => new { d.Post!.Category.Name, d.CreatedAt.Date })
                .Select(g => new
                {
                    CategoryName = g.Key.Name,
                    Date = g.Key.Date,
                    Amount = g.Sum(d => d.Amount)
                })
                .ToListAsync();

            var categories = await _context.Categories.Select(c => c.Name).ToListAsync();
            var categoryTrends = new List<CategoryTrendDto>();

            foreach (var categoryName in categories)
            {
                var trends = Enumerable.Range(0, daysCount)
                    .Select(i => startDate.AddDays(i))
                    .Select(date => new TrendDto
                    {
                        Date = date,
                        Value = categoryTrendsRaw
                            .FirstOrDefault(ct => ct.CategoryName == categoryName && ct.Date == date)?.Amount ?? 0
                    })
                    .ToList();

                categoryTrends.Add(new CategoryTrendDto
                {
                    CategoryName = categoryName,
                    Trends = trends
                });
            }

            var analysisDto = new AnalysisToReturnDto
            {
                TotalPaidAndProcessedDonations = totalPaidAndProcessedDonations,
                TotalPaidDonations = totalPaidDonations,
                TotalProcessedToClientDonations = totalProcessedToClientDonations,
                TotalCompletedTargets = totalCompletedTargets,
                TotalDonationAmount = totalDonationAmount,
                TotalDonors = totalDonors,
                DonationTrend = fullTrend,
                CategoryTrends = categoryTrends,
                CategoryBreakdown = categoryBreakdown,
                StatusBreakdown = statusBreakdown
            };

            return Result<AnalysisToReturnDto>.Ok(analysisDto);
        }

    }
}
