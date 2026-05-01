using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmartDonationSystem.Core.Modules.User.UserAnalysis.DTOs;
using SmartDonationSystem.Core.Modules.User.UserAnalysis.Interfaces;
using SmartDonationSystem.DataAccess;
using SmartDonationSystem.Shared.Enums;
using SmartDonationSystem.Shared.Responses;
using System.Security.Claims;

namespace SmartDonationSystem.Services.Modules.User.UserAnalysis
{
    public class UserAnalysisService : IUserAnalysisService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserAnalysisService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<UserAnalysisDto>> GetUserAnalysisAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Result<UserAnalysisDto>.Unauthorized("User not found.");
            }

            var result = new UserAnalysisDto();
            var successfulDonationStatuses = new[] { DonationStatus.Paid.ToString(), DonationStatus.Processed.ToString() };
            var thirtyDaysAgo = DateTime.UtcNow.Date.AddDays(-30);

            // --- Donor Impact ---
            var userDonations = await _context.Donations
                .Include(d => d.Post)
                .ThenInclude(p => p.Category)
                .Where(d => d.DonorId == userId && successfulDonationStatuses.Contains(d.Status))
                .ToListAsync();

            result.DonorImpact.TotalDonated = userDonations.Sum(d => d.Amount);
            result.DonorImpact.TotalCausesSupported = userDonations.Select(d => d.PostId).Distinct().Count();

            result.DonorImpact.CategoriesSupported = userDonations
                .Where(d => d.Post != null)
                .GroupBy(d => d.Post!.Category.Name)
                .Select(g => new CategoryDistributionDto
                {
                    CategoryName = g.Key,
                    TotalAmount = g.Sum(d => d.Amount),
                    DonationCount = g.Count()
                })
                .ToList();

            var donationTrendRaw = userDonations
                .Where(d => d.CreatedAt.Date >= thirtyDaysAgo)
                .GroupBy(d => d.CreatedAt.Date)
                .ToDictionary(g => g.Key, g => g.Sum(d => d.Amount));

            result.DonorImpact.DonationTrend = Enumerable.Range(0, 31)
                .Select(i => thirtyDaysAgo.AddDays(i))
                .Select(date => new TrendDto
                {
                    Date = date,
                    Value = donationTrendRaw.ContainsKey(date) ? donationTrendRaw[date] : 0
                })
                .ToList();

            // --- Requester Impact ---
            var userPosts = await _context.Posts
                .Include(p => p.Donations)
                .Where(p => p.ApplicationUserId == userId)
                .ToListAsync();

            var activeStatuses = new[] { PostStatus.Pending.ToString(), PostStatus.Approved.ToString() };
            result.RequesterImpact.ActiveNeeds = userPosts.Count(p => activeStatuses.Contains(p.Status));

            var fulfilledPosts = userPosts.Where(p => p.TargetMoney != null && p.TargetMoney > 0 &&
                p.Donations!.Where(d => successfulDonationStatuses.Contains(d.Status)).Sum(d => d.Amount) >= p.TargetMoney);
            result.RequesterImpact.TotalNeedsFulfilled = fulfilledPosts.Count();

            var allReceivedDonations = await _context.Donations
                .Where(d => d.Post != null && d.Post.ApplicationUserId == userId && successfulDonationStatuses.Contains(d.Status))
                .ToListAsync();

            result.RequesterImpact.TotalFundsRaised = allReceivedDonations.Sum(d => d.Amount);

            var raisedTrendRaw = allReceivedDonations
                .Where(d => d.CreatedAt.Date >= thirtyDaysAgo)
                .GroupBy(d => d.CreatedAt.Date)
                .ToDictionary(g => g.Key, g => g.Sum(d => d.Amount));

            result.RequesterImpact.FundsRaisedTrend = Enumerable.Range(0, 31)
                .Select(i => thirtyDaysAgo.AddDays(i))
                .Select(date => new TrendDto
                {
                    Date = date,
                    Value = raisedTrendRaw.ContainsKey(date) ? raisedTrendRaw[date] : 0
                })
                .ToList();

            return Result<UserAnalysisDto>.Ok(result);
        }
        public async Task<Result<PlatformAnalysisDto>> GetPlatformAnalysisAsync()
        {
            var result = new PlatformAnalysisDto();
            var successfulDonationStatuses = new[] { DonationStatus.Paid.ToString(), DonationStatus.Processed.ToString() };
            var thirtyDaysAgo = DateTime.UtcNow.Date.AddDays(-30);

            var allDonations = await _context.Donations
                .Where(d => successfulDonationStatuses.Contains(d.Status))
                .ToListAsync();

            result.TotalDonationsProcessed = allDonations.Sum(d => d.Amount);
            result.TotalSuccessfulTransactions = allDonations.Count;
            result.TotalDonors = allDonations.Select(d => d.DonorId).Distinct().Count();

            var allPosts = await _context.Posts
                .Include(p => p.Donations)
                .Include(p => p.Category)
                .ToListAsync();

            result.TotalRequesters = allPosts.Select(p => p.ApplicationUserId).Distinct().Count();

            var activeStatuses = new[] { PostStatus.Pending.ToString(), PostStatus.Approved.ToString() };
            result.TotalActiveCauses = allPosts.Count(p => activeStatuses.Contains(p.Status));

            result.TotalCausesFulfilled = allPosts.Count(p => p.TargetMoney != null && p.TargetMoney > 0 &&
                p.Donations!.Where(d => successfulDonationStatuses.Contains(d.Status)).Sum(d => d.Amount) >= p.TargetMoney);

            result.TopCategories = allDonations
                .Where(d => d.Post != null)
                .GroupBy(d => d.Post!.Category.Name)
                .Select(g => new CategoryDistributionDto
                {
                    CategoryName = g.Key,
                    TotalAmount = g.Sum(d => d.Amount),
                    DonationCount = g.Count()
                })
                .OrderByDescending(c => c.TotalAmount)
                .Take(5)
                .ToList();

            var growthTrendRaw = allDonations
                .Where(d => d.CreatedAt.Date >= thirtyDaysAgo)
                .GroupBy(d => d.CreatedAt.Date)
                .ToDictionary(g => g.Key, g => g.Sum(d => d.Amount));

            result.PlatformGrowthTrend = Enumerable.Range(0, 31)
                .Select(i => thirtyDaysAgo.AddDays(i))
                .Select(date => new TrendDto
                {
                    Date = date,
                    Value = growthTrendRaw.ContainsKey(date) ? growthTrendRaw[date] : 0
                })
                .ToList();

            return Result<PlatformAnalysisDto>.Ok(result);
        }
    }
}
