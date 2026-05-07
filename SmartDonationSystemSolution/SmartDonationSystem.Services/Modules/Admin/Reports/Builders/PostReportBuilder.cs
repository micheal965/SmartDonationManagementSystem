using Microsoft.EntityFrameworkCore;
using SmartDonationSystem.Core.Modules.Admin.ReportManagement.DTOs;
using SmartDonationSystem.Core.Modules.Admin.ReportManagement.Enums;
using SmartDonationSystem.DataAccess;
using SmartDonationSystem.Services.Modules.Reports.DTOs;
using SmartDonationSystem.Services.Modules.Reports.Interfaces;
using SmartDonationSystem.Shared.Enums;

namespace SmartDonationSystem.Services.Modules.Admin.Reports.Builders
{
    public class PostReportBuilder(ApplicationDbContext _context) : IReportBuilder
    {
        public ReportType SupportedType => ReportType.PostsReport;

        public async Task<ReportDocumentModel> BuildAsync(ReportRequest request)
        {
            var query = _context.Posts
                .Include(p => p.Category)
                .Include(p => p.Donations)
                .AsQueryable();

            if (request.DateFrom.HasValue)
                query = query.Where(p => p.CreatedAt >= request.DateFrom.Value);
            if (request.DateTo.HasValue)
                query = query.Where(p => p.CreatedAt <= request.DateTo.Value);

            foreach (var filter in request.Filters)
            {
                query = ApplyFilter(query, filter);
            }

            var totalCount = await query.CountAsync();

            var rawData = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var data = rawData.Select(p => new PostReportRowDto
            {
                Title = p.Title,
                Category = p.Category.Name,
                TargetMoney = p.TargetMoney ?? 0,
                CollectedMoney = p.Donations?.Where(d => d.Status == DonationStatus.Paid.ToString() || d.Status == DonationStatus.Processed.ToString()).Sum(d => d.Amount) ?? 0,
                Status = p.Status,
                CreatedAt = p.CreatedAt,
                CreatedByRole = p.CreatedByRole
            }).ToList();

            return new ReportDocumentModel
            {
                Title = "Donation Posts Report",
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                Headers = new List<string> { "Title", "Target", "Collected", "Status", "Created At", "Category", "CreatedBy" },
                Rows = data.Select(p => new List<string>
                {
                    p.Title,
                    p.TargetMoney.ToString("C"),
                    p.CollectedMoney.ToString("C"),
                    p.Status,
                    p.CreatedAt.ToString("yyyy-MM-dd"),
                    p.Category,
                    p.CreatedByRole
                }).ToList(),
                Summary = new Dictionary<string, string>
                {
                    { "Total Posts", totalCount.ToString() },
                    { "Approved Posts", (await query.CountAsync(p => p.Status == PostStatus.Approved.ToString())).ToString() }
                }
            };
        }

        private IQueryable<Core.Common.Models.Post> ApplyFilter(
            IQueryable<Core.Common.Models.Post> query, ReportFilter filter)
        {
            return filter.Field switch
            {
                "Status" => filter.Operator switch
                {
                    "eq" => query.Where(p => p.Status == filter.Value),
                    _ => query
                },
                "CategoryName" => filter.Operator switch
                {
                    "contains" => query.Where(p => p.Category.Name.Contains(filter.Value)),
                    "eq" => query.Where(p => p.Category.Name == filter.Value),
                    _ => query
                },
                "CreatedByRole" => filter.Operator switch
                {
                    "eq" => query.Where(p => p.CreatedByRole == filter.Value),
                    _ => query
                },
                "TargetMoney" => decimal.TryParse(filter.Value, out var val) ? filter.Operator switch
                {
                    "eq" => query.Where(p => p.TargetMoney == val),
                    "gt" => query.Where(p => p.TargetMoney > val),
                    "lt" => query.Where(p => p.TargetMoney < val),
                    _ => query
                } : query,
                "CreatedAt" => DateTime.TryParse(filter.Value, out var date) ? filter.Operator switch
                {
                    "eq" => query.Where(p => p.CreatedAt.Date == date.Date),
                    "gt" => query.Where(p => p.CreatedAt > date),
                    "lt" => query.Where(p => p.CreatedAt < date),
                    _ => query
                } : query,
                _ => query
            };
        }
    }
}
