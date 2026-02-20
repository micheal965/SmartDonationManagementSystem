using System.Text.Json.Serialization;

namespace SmartDonationSystem.Core.Modules.AI.DTOs
{
    public class GeminiResponse
    {
        [JsonPropertyName("candidates")]
        public List<GeminiCandidate>? Candidates { get; set; }

        // helper property
        [JsonIgnore]
        public string CompletionText => Candidates?.FirstOrDefault()?.Content ?? string.Empty;
    }
    public class GeminiCandidate
    {
        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;
    }
}
