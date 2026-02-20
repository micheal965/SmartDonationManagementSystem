namespace SmartDonationSystem.Core.Modules.AI
{
    public interface IAIClient
    {
        Task<string> GenerateAsync(string prompt);
    }
}
