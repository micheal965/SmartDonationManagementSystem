using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SmartDonationSystem.Core.Modules.Admin.ReportManagement.DTOs;

namespace SmartDonationSystem.Services.Modules.Admin.Reports.PdfGeneration
{
    public class QuestPdfGenerator
    {
        public QuestPdfGenerator()
        {
            // QuestPDF Community License
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public byte[] GenerateReport(ReportDocumentModel model, string? logoPath = null)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Verdana));

                    page.Header().Element(header => ComposeHeader(header, model, logoPath));
                    page.Content().Element(content => ComposeContent(content, model));
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Page ");
                        x.CurrentPageNumber();
                        x.Span(" of ");
                        x.TotalPages();
                    });
                });
            }).GeneratePdf();
        }

        private void ComposeHeader(IContainer container, ReportDocumentModel model, string? logoPath)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text(model.Title).FontSize(24).SemiBold().FontColor(Colors.Indigo.Medium);
                    col.Item().Text(text =>
                    {
                        text.Span("Generated on: ").SemiBold();
                        text.Span($"{model.GeneratedAt:yyyy-MM-dd HH:mm}");
                    });
                });

                if (!string.IsNullOrEmpty(logoPath) && File.Exists(logoPath))
                {
                    row.ConstantItem(100).Height(50).Image(logoPath);
                }
                else
                {
                    row.ConstantItem(100).Height(50).Placeholder();
                }
            });
        }

        private void ComposeContent(IContainer container, ReportDocumentModel model)
        {
            container.PaddingVertical(10).Column(column =>
            {
                column.Spacing(10);

                if (model.Summary != null && model.Summary.Count > 0)
                {
                    column.Item().Border(1).BorderColor(Colors.Grey.Lighten2).Background(Colors.Grey.Lighten4).Padding(10).Row(row =>
                    {
                        foreach (var item in model.Summary)
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text(item.Key).FontSize(8).FontColor(Colors.Grey.Medium);
                                c.Item().Text(item.Value).FontSize(12).SemiBold();
                            });
                        }
                    });
                }

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        for (int i = 0; i < model.Headers.Count; i++)
                        {
                            columns.RelativeColumn();
                        }
                    });

                    table.Header(header =>
                    {
                        foreach (var h in model.Headers)
                        {
                            header.Cell().Background(Colors.Indigo.Medium).Padding(5).Text(h).FontColor(Colors.White).SemiBold();
                        }
                    });

                    foreach (var rowData in model.Rows)
                    {
                        foreach (var cellData in rowData)
                        {
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(5).Text(cellData);
                        }
                    }
                });

                column.Item().AlignRight().Text($"Total Records: {model.TotalCount}").FontSize(9).Italic();
            });
        }
    }
}
