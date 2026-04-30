using Microsoft.EntityFrameworkCore;
using SmartDonationSystem.Core.Modules.User.Sidebar.DTOs;
using SmartDonationSystem.Core.Modules.User.Sidebar.Interfaces;
using SmartDonationSystem.DataAccess;
using SmartDonationSystem.Shared.Enums;
using SmartDonationSystem.Shared.Responses;

namespace SmartDonationSystem.Services.Modules.User.Sidebar
{
    public class SidebarService : ISidebarService
    {
        private readonly ApplicationDbContext _context;

        public SidebarService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<SidebarDataDto>> GetSidebarDataAsync()
        {
            var data = new SidebarDataDto();

            // 1. Live Impact (Recent 3 donations)
            data.LiveImpacts = await _context.Donations
                .Include(d => d.Donor)
                .Include(d => d.Post)
                .Where(d => d.Status == DonationStatus.Paid.ToString() || d.Status == DonationStatus.Processed.ToString())
                .OrderByDescending(d => d.CreatedAt)
                .Take(3)
                .Select(d => new LiveImpactDto
                {
                    DonorName = d.Donor.FullName,
                    DonorPicture = d.Donor.PictureUrl,
                    Amount = d.Amount,
                    PostTitle = d.Post != null ? d.Post.Title : "Platform",
                    CreatedAt = d.CreatedAt
                })
                .ToListAsync();

            // 2. Trending Needs (Top 2 posts by Most Viewed / AnalyticsEvents Count)
            data.TrendingNeeds = await _context.Posts
                .Include(p => p.Category)
                .Where(p => p.Status == PostStatus.Approved.ToString())
                .OrderByDescending(p => p.AnalyticsEvents.Count)
                .Take(2)
                .Select(p => new TrendingNeedDto
                {
                    PostId = p.Id,
                    Title = p.Title,
                    CategoryName = p.Category.Name,
                    PriorityLevel = p.PriorityLevel
                })
                .ToListAsync();

            // 3. Total Impact Today
            var today = DateTime.UtcNow.Date;
            data.TotalImpact.TotalAmountToday = await _context.Donations
                .Where(d => d.CreatedAt >= today && (d.Status == DonationStatus.Paid.ToString() || d.Status == DonationStatus.Processed.ToString()))
                .SumAsync(d => d.Amount);

            data.TotalImpact.VerifiedCasesCount = await _context.Posts
                .Where(p => p.Status == PostStatus.Approved.ToString())
                .CountAsync();

            return Result<SidebarDataDto>.Ok(data);
        }
    }
}