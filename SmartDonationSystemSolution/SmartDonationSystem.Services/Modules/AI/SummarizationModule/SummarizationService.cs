using SmartDonationSystem.Core.Modules.AI;

namespace SmartDonationSystem.Services.Modules.AI.SummarizationModule
{
    public class SummarizationService
    {
        private readonly IAIClient _aiClient;
        private readonly PromptBuilder _promptBuilder;
        public SummarizationService(IAIClient aiClient, PromptBuilder promptBuilder)
        {
            _aiClient = aiClient;
            _promptBuilder = promptBuilder;
        }
        public async Task<string> GenerateSummaryAsync(string text)
        {
            var chunkingService = new ChunkingService();
            var chunks = chunkingService.ChunkText(text);
            var summaries = new List<string>();

            foreach (var chunk in chunks)
            {
                var prompt = _promptBuilder.BuildSummaryPrompt(chunk);
                var summary = await _aiClient.GenerateAsync(prompt);
                summaries.Add(summary);
            }

            return string.Join(" ", summaries);
        }
    }
}
