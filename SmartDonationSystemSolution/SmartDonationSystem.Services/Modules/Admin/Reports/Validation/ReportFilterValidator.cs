using SmartDonationSystem.Core.Modules.Admin.ReportManagement.DTOs;
using SmartDonationSystem.Core.Modules.Admin.ReportManagement.Enums;

namespace SmartDonationSystem.Services.Modules.Admin.Reports.Validation
{
    public static class ReportFilterValidator
    {
        private static readonly Dictionary<ReportType, HashSet<string>> WhitelistedFields = new()
        {
            {
                ReportType.DonationsReport, new HashSet<string>
                {
                    "Status", "Type", "PaymentGateway", "Amount", "DonorName","CreatedAt"
                }
            },
            {
                ReportType.PostsReport, new HashSet<string>
                {
                    "Status", "CategoryName", "CreatedByRole", "TargetMoney", "CreatedAt"
                }
            },
            {
                ReportType.UsersReport, new HashSet<string>
                {
                    "FullName", "PhoneNumber", "IsDeleted", "BirthDate"
                }
            }
        };

        private static readonly HashSet<string> AllowedOperators = new()
        {
            "eq", "neq", "gt", "lt", "contains"
        };

        public static bool IsValid(ReportRequest request, out string? errorMessage)
        {
            if (!WhitelistedFields.TryGetValue(request.ReportType, out var allowedFields))
            {
                errorMessage = "Unsupported report type.";
                return false;
            }

            foreach (var filter in request.Filters)
            {
                if (!allowedFields.Contains(filter.Field))
                {
                    errorMessage = $"Field '{filter.Field}' is not allowed for {request.ReportType}.";
                    return false;
                }

                if (!AllowedOperators.Contains(filter.Operator.ToLower()))
                {
                    errorMessage = $"Operator '{filter.Operator}' is not supported.";
                    return false;
                }
            }

            errorMessage = null;
            return true;
        }
    }
}
