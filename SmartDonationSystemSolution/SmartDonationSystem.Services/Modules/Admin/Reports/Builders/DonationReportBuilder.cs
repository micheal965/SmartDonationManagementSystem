using Microsoft.EntityFrameworkCore;
using SmartDonationSystem.Core.Common.Models;
using SmartDonationSystem.Core.Modules.Admin.ReportManagement.DTOs;
using SmartDonationSystem.Core.Modules.Admin.ReportManagement.Enums;
using SmartDonationSystem.DataAccess;
using SmartDonationSystem.Services.Modules.Reports.DTOs;
using SmartDonationSystem.Services.Modules.Reports.Interfaces;
using SmartDonationSystem.Shared.Enums;

namespace SmartDonationSystem.Services.Modules.Admin.Reports.Builders
{
    public class DonationReportBuilder(ApplicationDbContext _context) : IReportBuilder
    {
        public ReportType SupportedType => ReportType.DonationsReport;

        public async Task<ReportDocumentModel> BuildAsync(ReportRequest request)
        {
            var query = _context.Donations
                .Include(d => d.Donor)
                .Include(d => d.Post)
                .AsQueryable();

            // Apply Date Range
            if (request.DateFrom.HasValue)
                query = query.Where(d => d.CreatedAt >= request.DateFrom.Value);
            if (request.DateTo.HasValue)
                query = query.Where(d => d.CreatedAt <= request.DateTo.Value);

            // Apply Filters
            foreach (var filter in request.Filters)
            {
                query = ApplyFilter(query, filter);
            }

            var totalCount = await query.CountAsync();

            var data = await query
                .OrderByDescending(d => d.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(d => new DonationReportRowDto
                {
                    DonorName = d.Donor.FullName,
                    PostTitle = d.Post != null ? d.Post.Title : "Platform",
                    Amount = d.Amount,
                    Status = d.Status,
                    PaymentGateway = d.PaymentGateway,
                    CreatedAt = d.CreatedAt,
                    RequesterName = d.Post != null && d.Post.CreatedByRole == AppRoles.Requester && d.Post.ApplicationUser != null ? d.Post.ApplicationUser.FullName : "N/A"
                })
                .ToListAsync();

            return new ReportDocumentModel
            {
                Title = "Donations Report",
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                Headers = new List<string> { "Donor", "Title", "Amount", "Status", "Gateway", "Requester", "Date" },
                Rows = data.Select(d => new List<string>
                {
                    d.DonorName,
                    d.PostTitle,
                    d.Amount.ToString("C"),
                    d.Status,
                    d.PaymentGateway,
                    d.RequesterName,
                    d.CreatedAt.ToString("yyyy-MM-dd HH:mm")
                }).ToList(),
                Summary = new Dictionary<string, string>
                {
                    { "Total Amount", (await query.SumAsync(d => d.Amount)).ToString("C") },
                    { "Total Records", totalCount.ToString() }
                }
            };
        }

        private IQueryable<Donation> ApplyFilter(
            IQueryable<Donation> query, ReportFilter filter)
        {
            return filter.Field switch
            {
                "Status" => filter.Operator switch
                {
                    "eq" => query.Where(d => d.Status == filter.Value),
                    "contains" => query.Where(d => d.Status.Contains(filter.Value)),
                    _ => query
                },
                "Type" => filter.Operator switch
                {
                    "eq" => query.Where(d => d.Type == filter.Value),
                    _ => query
                },
                "PaymentGateway" => filter.Operator switch
                {
                    "eq" => query.Where(d => d.PaymentGateway == filter.Value),
                    _ => query
                },
                "Amount" => decimal.TryParse(filter.Value, out var val) ? filter.Operator switch
                {
                    "eq" => query.Where(d => d.Amount == val),
                    "gt" => query.Where(d => d.Amount > val),
                    "lt" => query.Where(d => d.Amount < val),
                    _ => query
                } : query,
                "DonorName" => filter.Operator switch
                {
                    "contains" => query.Where(d => d.Donor.FullName.Contains(filter.Value)),
                    "eq" => query.Where(d => d.Donor.FullName == filter.Value),
                    _ => query
                },
                "CreatedAt" => DateTime.TryParse(filter.Value, out var date) ? filter.Operator switch
                {
                    "eq" => query.Where(d => d.CreatedAt.Date == date.Date),
                    "gt" => query.Where(d => d.CreatedAt > date),
                    "lt" => query.Where(d => d.CreatedAt < date),
                    _ => query
                } : query,
                _ => query
            };
        }
    }
}
