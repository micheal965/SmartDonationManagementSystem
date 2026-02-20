namespace SmartDonationSystem.Services.Modules.AI.SummarizationModule
{
    public class PromptBuilder
    {
        public string BuildSummaryPrompt(string text)
        {
            return $"Please summarize the following content in a concise way:\n{text}";
        }
    }
}
