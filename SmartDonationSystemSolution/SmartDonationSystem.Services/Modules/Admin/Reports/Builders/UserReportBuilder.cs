using Microsoft.EntityFrameworkCore;
using SmartDonationSystem.Core.Modules.Admin.ReportManagement.DTOs;
using SmartDonationSystem.Core.Modules.Admin.ReportManagement.Enums;
using SmartDonationSystem.DataAccess;
using SmartDonationSystem.Services.Modules.Reports.Interfaces;

namespace SmartDonationSystem.Services.Modules.Admin.Reports.Builders
{
    public class UserReportBuilder(ApplicationDbContext _context) : IReportBuilder
    {
        public ReportType SupportedType => ReportType.UsersReport;

        public async Task<ReportDocumentModel> BuildAsync(ReportRequest request)
        {
            var query = from u in _context.Users
                        join ur in _context.UserRoles on u.Id equals ur.UserId
                        join r in _context.Roles on ur.RoleId equals r.Id
                        select new UserReportQueryResult
                        {
                            FullName = u.FullName,
                            PhoneNumber = u.PhoneNumber,
                            Role = r.Name,
                            IsSoftDeleted = u.IsSoftDeleted,
                            BirthDate = u.BirthDate
                        };

            foreach (var filter in request.Filters)
            {
                query = ApplyFilter(query, filter);
            }

            var totalCount = await query.CountAsync();

            var data = await query
                .OrderBy(u => u.FullName)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new ReportDocumentModel
            {
                Title = "Users Report",
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                Headers = new List<string> { "Full Name", "PhoneNumber", "Role", "Status", "Birth Date" },
                Rows = data.Select(u => new List<string>
                {
                    u.FullName,
                    u.PhoneNumber ?? "N/A",
                    u.Role ?? "N/A",
                    u.IsSoftDeleted ? "Deactivated" : "Active",
                    u.BirthDate.ToString("yyyy-MM-dd")
                }).ToList(),
                Summary = new Dictionary<string, string>
                {
                    { "Total Users", totalCount.ToString() },
                    { "Active Users", (await query.CountAsync(u => !u.IsSoftDeleted)).ToString() }
                }
            };
        }

        private IQueryable<UserReportQueryResult> ApplyFilter(IQueryable<UserReportQueryResult> query, ReportFilter filter)
        {
            return filter.Field switch
            {
                "FullName" => filter.Operator switch
                {
                    "contains" => query.Where(u => u.FullName.Contains(filter.Value)),
                    "eq" => query.Where(u => u.FullName == filter.Value),
                    _ => query
                },
                "PhoneNumber" => filter.Operator switch
                {
                    "contains" => query.Where(u => u.PhoneNumber != null && u.PhoneNumber.Contains(filter.Value)),
                    "eq" => query.Where(u => u.PhoneNumber == filter.Value),
                    _ => query
                },
                "IsDeleted" => bool.TryParse(filter.Value, out var val) ? query.Where(u => u.IsSoftDeleted == val) : query,
                "BirthDate" => DateOnly.TryParse(filter.Value, out var date) ? filter.Operator switch
                {
                    "eq" => query.Where(u => u.BirthDate == date),
                    "gt" => query.Where(u => u.BirthDate > date),
                    "lt" => query.Where(u => u.BirthDate < date),
                    _ => query
                } : query,
                _ => query
            };
        }

        private class UserReportQueryResult
        {
            public string FullName { get; set; } = null!;
            public string? PhoneNumber { get; set; }
            public string? Role { get; set; }
            public bool IsSoftDeleted { get; set; }
            public DateOnly BirthDate { get; set; }
        }
    }
}

