namespace SmartDonationSystem.Core.Modules.FileExtractionModule
{
    public interface IFileExtractionService
    {
        bool CanExtract(string fileExtension);
        string ExtractText(Stream fileStream);
    }
}
