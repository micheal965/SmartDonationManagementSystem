using Microsoft.Extensions.Configuration;
using SmartDonationSystem.Core.Modules.AI;
using System.Text;
using System.Text.Json;

namespace SmartDonationSystem.Services.Modules.AI
{
    public class GeminiClient : IAIClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public GeminiClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<string> GenerateAsync(string prompt)
        {
            var apiKey = _configuration["Gemini:ApiKey"];
            var model = _configuration["Gemini:Model"];

            var url = $"https://generativelanguage.googleapis.com/v1/models/{model}:generateContent";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("x-goog-api-key", apiKey);

            var body = new
            {
                contents = new[]
                {
            new
            {
                parts = new[]
                {
                    new { text = prompt }
                }
            }
                }
            };

            var content = new StringContent(
                JsonSerializer.Serialize(body),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync(url, content);

            var responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Response Body" + responseBody);

            response.EnsureSuccessStatusCode();

            return responseBody;
        }

    }
}
