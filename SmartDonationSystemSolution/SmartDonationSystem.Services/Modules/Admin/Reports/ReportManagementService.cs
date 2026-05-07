using SmartDonationSystem.Core.Modules.Admin.ReportManagement.DTOs;
using SmartDonationSystem.Core.Modules.Admin.ReportManagement.Interfaces;
using SmartDonationSystem.Services.Modules.Admin.Reports.PdfGeneration;
using SmartDonationSystem.Services.Modules.Admin.Reports.Validation;
using SmartDonationSystem.Services.Modules.Reports.Interfaces;
using SmartDonationSystem.Shared.Responses;

namespace SmartDonationSystem.Services.Modules.Admin.Reports
{
    public class ReportManagementService(
        IEnumerable<IReportBuilder> _builders,
        QuestPdfGenerator _pdfGenerator) : IReportService
    {
        public async Task<Result<byte[]>> GeneratePdfAsync(ReportRequest request, string? logoPath = null)
        {
            // 1. Validate
            if (!ReportFilterValidator.IsValid(request, out var errorMessage))
            {
                return Result<byte[]>.BadRequest(errorMessage ?? "Invalid request");
            }

            // 2. Pick Builder
            var builder = _builders.FirstOrDefault(b => b.SupportedType == request.ReportType);
            if (builder == null)
            {
                return Result<byte[]>.BadRequest("No builder found for this report type.");
            }

            // 3. Build Model
            try
            {
                var reportModel = await builder.BuildAsync(request);

                // 4. Generate PDF
                var pdfBytes = _pdfGenerator.GenerateReport(reportModel, logoPath);

                return Result<byte[]>.Ok(pdfBytes);
            }
            catch (Exception ex)
            {
                // In a real app, log the exception
                return Result<byte[]>.ServerError($"Error generating report: {ex.Message}");
            }
        }
    }
}
