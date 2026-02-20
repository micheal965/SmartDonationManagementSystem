using Microsoft.EntityFrameworkCore;
using SmartDonationSystem.Core.Common.Models;
using SmartDonationSystem.Core.Modules.AI.DTOs;
using SmartDonationSystem.DataAccess;
using SmartDonationSystem.Services.Modules.AI.SummarizationModule;
using SmartDonationSystem.Shared.Enums;
using System.Text.Json;

namespace SmartDonationSystem.Services.Modules.AI.ClassificationScoringModule
{
    public class PostClassifierService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly GeminiClient _gemini;

        public PostClassifierService(GeminiClient gemini, SummarizationService summarizer, ApplicationDbContext dbContext)
        {
            _gemini = gemini;
            _dbContext = dbContext;
        }

        public async Task RunClassificationJobAsync()
        {
            var postsToScore = await _dbContext.Posts
                .Where(p => (p.Status == PostStatus.Approved.ToString()) && (p.LastScoredAt == null || p.LastScoredAt < DateTime.UtcNow.AddMinutes(-10)))
                .ToListAsync();

            foreach (var post in postsToScore)
                await ClassifyPostAsync(post);

            await _dbContext.SaveChangesAsync();
        }

        // Classify a single post using Gemini AI
        private async Task ClassifyPostAsync(Post post)
        {
            // Build the prompt
            var prompt = $@"
                        You are an AI expert in donation management systems.

                        Your task is to classify the donation post and provide an impact score and priority level. Focus on the AI-generated summary, title, and content.

                        Post Details:
                        Title: {post.Title}
                        Content: {post.Content}
                        AI Summary: {post.AiSummary}

                        Respond ONLY in JSON like this example:
                        {{ ""ImpactScore"": 85, ""PriorityLevel"": 3 }}
                        ";

            // Send to Gemini
            var responseBody = await _gemini.GenerateAsync(prompt);
            var responseText = ExtractScoreFromResponse(responseBody);
            var result = JsonSerializer.Deserialize<ClassificationResult>(responseText);

            if (result != null)
            {
                post.ImpactScore = result.ImpactScore;
                post.PriorityLevel = result.PriorityLevel;
                post.LastScoredAt = DateTime.UtcNow;
            }
            if (post.PriorityLevel < 1) post.PriorityLevel = 1;
            if (post.PriorityLevel > 5) post.PriorityLevel = 5;
        }
        private string ExtractScoreFromResponse(string responseBody)
        {
            using var doc = JsonDocument.Parse(responseBody);

            // جلب الـ object داخل parts
            var partObject = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0];

            // جلب الـ text
            var responseText = partObject.GetProperty("text").GetString() ?? "";

            // إزالة Markdown ```json و ```
            var jsonText = responseText
                .Replace("```json", "")
                .Replace("```", "")
                .Trim();

            return jsonText;
        }
    }
}
