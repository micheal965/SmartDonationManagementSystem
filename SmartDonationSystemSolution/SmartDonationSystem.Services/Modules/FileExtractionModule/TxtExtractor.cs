using SmartDonationSystem.Core.Modules.FileExtractionModule;
using System.Text.RegularExpressions;

namespace SmartDonationSystem.Services.Modules.FileExtractionModule
{
    public class TxtExtractor : IFileExtractionService
    {
        public bool CanExtract(string fileExtension) => fileExtension.Equals(".txt", StringComparison.OrdinalIgnoreCase);

        public string ExtractText(Stream fileStream)
        {
            using var reader = new StreamReader(fileStream);
            string text = reader.ReadToEnd();

            string cleanedText = Regex.Replace(text, @"\s+", " ").Trim();

            return cleanedText;
        }
    }
}
