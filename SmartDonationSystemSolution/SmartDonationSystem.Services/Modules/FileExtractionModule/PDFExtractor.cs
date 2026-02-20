using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using SmartDonationSystem.Core.Modules.FileExtractionModule;
using System.Text;
using System.Text.RegularExpressions;

namespace SmartDonationSystem.Services.Modules.FileExtractionModule
{
    public class PDFExtractor : IFileExtractionService
    {
        public bool CanExtract(string fileExtension) => fileExtension.Equals(".pdf", StringComparison.OrdinalIgnoreCase);

        public string ExtractText(Stream pdfStream)
        {
            using var pdfDoc = new PdfDocument(new PdfReader(pdfStream));
            var strategy = new SimpleTextExtractionStrategy();
            var text = new StringBuilder();

            for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
            {
                string pageText = PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(i), strategy);
                text.AppendLine(pageText);
            }

            // Normalize all whitespace to single spaces
            string cleanedText = Regex.Replace(text.ToString(), @"\s+", " ").Trim();

            return cleanedText;
        }
    }
}
