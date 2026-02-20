using SmartDonationSystem.DataAccess;
using System.Text.Json;

namespace SmartDonationSystem.Services.Modules.AI.SummarizationModule
{
    public class SummaryJob
    {
        private readonly ApplicationDbContext _db;
        private readonly SummarizationService _summarizer;

        public SummaryJob(ApplicationDbContext db, SummarizationService summarizer)
        {
            _db = db;
            _summarizer = summarizer;
        }

        public async Task GenerateSummaryAsync(int postId)
        {
            var post = await _db.Posts.FindAsync(postId);
            if (post == null) return;

            var responseBody = await _summarizer.GenerateSummaryAsync(post.AiSummary);

            post.AiSummary = ExtractTextFromResponse(responseBody);
            await _db.SaveChangesAsync();
        }
        private string ExtractTextFromResponse(string responseBody)
        {
            using var doc = JsonDocument.Parse(responseBody);

            var text = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString() ?? "";

            return text!;
        }
    }
}
