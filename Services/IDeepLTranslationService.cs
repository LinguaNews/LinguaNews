using LinguaNews.Options;
using LinguaNews.Models;  
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;
using System.Net.Http.Json;


namespace LinguaNews.Services
{
    public interface ITranslationService
    {
        Task<string> TranslateAsync(
            string text,
            string targetLang);
    }
}

namespace LinguaNews.Services
{
    public class DeepLTranslationService : ITranslationService
    {
        private readonly HttpClient _httpClient;
        private readonly DeepLOptions _options;

        public DeepLTranslationService(HttpClient httpClient, IOptions<DeepLOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<string> TranslateAsync(string text, string targetLang)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;
            if (string.IsNullOrWhiteSpace(_options.ApiKey)) return "Error: API Key Missing";

            // DeepL Requirement: Target language must be uppercase (e.g., "ES")
            var cleanLang = targetLang.ToUpperInvariant();

            var requestBody = new
            {
                text = new[] { text },
                target_lang = cleanLang
            };

            var request = new HttpRequestMessage(HttpMethod.Post, _options.BaseUrl);
            request.Headers.Add("Authorization", $"DeepL-Auth-Key {_options.ApiKey}");
            request.Content = JsonContent.Create(requestBody);

            try
            {
                using var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                    return $"DeepL Error: {response.StatusCode}";

                var result = await response.Content.ReadFromJsonAsync<DeepLResponse>();
                return result?.Translations?.FirstOrDefault()?.TranslatedText ?? "Error: Empty Response";
            }
            catch (Exception ex)
            {
                return $"System Error: {ex.Message}";
            }
        }
    }
}
