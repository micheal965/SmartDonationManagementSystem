using SmartDonationSystem.Core.Modules.FileExtractionModule;

namespace SmartDonationSystem.Services.Modules.FileExtractionModule
{
    public class FileExtractionOrchestrator
    {
        private readonly IEnumerable<IFileExtractionService> _extractors;

        public FileExtractionOrchestrator(IEnumerable<IFileExtractionService> extractors)
        {
            _extractors = extractors;
        }
        public async Task<string> ExtractTextFromUrlAsync(string fileUrl)
        {
            var ext = Path.GetExtension(fileUrl);
            var extractor = _extractors.FirstOrDefault(e => e.CanExtract(ext));

            if (extractor == null)
                throw new NotSupportedException($"File type {ext} not supported");

            await using var stream = await GetFileStreamFromUrlAsync(fileUrl);
            return extractor.ExtractText(stream);
        }

        private async Task<Stream> GetFileStreamFromUrlAsync(string fileUrl)
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(fileUrl);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Unable to fetch file from Cloudinary");

            return await response.Content.ReadAsStreamAsync();
        }
    }
}
