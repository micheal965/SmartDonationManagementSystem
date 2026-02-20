using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using SmartDonationSystem.Core.Modules.FileExtractionModule;
using System.Text.RegularExpressions;

namespace SmartDonationSystem.Services.Modules.FileExtractionModule
{
    public class DocxExtractor : IFileExtractionService
    {
        public bool CanExtract(string fileExtension) => fileExtension.Equals(".docx", StringComparison.OrdinalIgnoreCase);

        public string ExtractText(Stream fileStream)
        {
            using var doc = WordprocessingDocument.Open(fileStream, false);

            string cleanedText = string.Join(" ",
                doc.MainDocumentPart.Document.Body
                   .Descendants<Paragraph>()
                   .Select(p => p.InnerText.Trim())
                   .Where(t => !string.IsNullOrEmpty(t))
            );

            cleanedText = Regex.Replace(cleanedText, @"\s+", " ").Trim();

            return cleanedText;
        }
    }
}
