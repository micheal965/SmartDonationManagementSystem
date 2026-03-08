using Microsoft.EntityFrameworkCore;
using SmartDonationSystem.Core.Common.Models;
using SmartDonationSystem.Core.Modules.AI.DTOs;
using SmartDonationSystem.DataAccess;
using SmartDonationSystem.Services.Modules.AI.SummarizationModule;
using SmartDonationSystem.Shared.Enums;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

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

        public async Task RunClassificationJobByCategoryAsync()
        {
            var categories = await _dbContext.Categories.ToListAsync();

            foreach (var category in categories)
            {
                var postsToScore = await _dbContext.Posts
                    .Where(p => p.CategoryId == category.Id &&
                                p.Status == PostStatus.Approved.ToString() &&
                                (p.LastScoredAt == null || p.LastScoredAt < DateTime.UtcNow.AddMinutes(-10)))
                    .ToListAsync();

                // Classify all posts in this category at once
                await ClassifyPostsAsync(postsToScore, category.Name);

                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task ClassifyPostsAsync(List<Post> posts, string categoryName)
        {
            if (!posts.Any()) return;

            var promptBuilder = new StringBuilder();
            promptBuilder.AppendLine("You are an AI expert in donation management systems.");
            promptBuilder.AppendLine($"Your task is to classify posts in the '{categoryName}' category by importance.");
            promptBuilder.AppendLine("Use the following order of priority when evaluating each post:");
            promptBuilder.AppendLine("1. AI Summary (most important)");
            promptBuilder.AppendLine("2. Content (medium importance)");
            promptBuilder.AppendLine("3. Title (least importance)");
            promptBuilder.AppendLine("For each post, provide an ImpactScore (0-100) and PriorityLevel (1-5).");
            promptBuilder.AppendLine("Respond ONLY in JSON like this example:");
            promptBuilder.AppendLine(@"[{ ""PostId"": 123, ""ImpactScore"": 85, ""PriorityLevel"": 3 }, ...]");

            promptBuilder.AppendLine("\nPosts:");
            foreach (var post in posts)
            {
                promptBuilder.AppendLine($"Id: {post.Id}");
                promptBuilder.AppendLine($"AI Summary: {post.AiSummary}");
                promptBuilder.AppendLine($"Content: {post.Content}");
                promptBuilder.AppendLine($"Title: {post.Title}");
                promptBuilder.AppendLine();
            }

            var responseBody = await _gemini.GenerateAsync(promptBuilder.ToString());

            var responseText = ExtractScoreFromResponse(responseBody);

            var results = JsonSerializer.Deserialize<List<ClassificationResult>>(responseText);

            if (results != null)
            {
                foreach (var result in results)
                {
                    var post = posts.FirstOrDefault(p => p.Id == result.PostId);
                    if (post != null)
                    {
                        post.ImpactScore = result.ImpactScore;
                        post.PriorityLevel = Math.Clamp(result.PriorityLevel, 1, 5);
                        post.LastScoredAt = DateTime.UtcNow;
                    }
                }
            }
        }
        private string ExtractScoreFromResponse(string responseBody)
        {
            if (string.IsNullOrWhiteSpace(responseBody))
                return "[]";

            try
            {
                using var doc = JsonDocument.Parse(responseBody);

                var partText = doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString() ?? "";

                // Remove markdown ```json and ``` and trim
                var jsonText = partText
                    .Replace("```json", "")
                    .Replace("```", "")
                    .Trim();

                // Remove any extra line breaks
                jsonText = Regex.Replace(jsonText, @"^\s+|\s+$", "");
                jsonText = Regex.Replace(jsonText, @"\r\n|\r|\n", "");

                // Ensure it’s a valid JSON array (starts with [)
                if (!jsonText.StartsWith("["))
                {
                    Console.WriteLine("Warning: AI response does not start with '['. Returning empty array.");
                    return "[]";
                }

                return jsonText;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to extract JSON from AI response: " + ex.Message);
                return "[]";
            }
        }
    }
}
