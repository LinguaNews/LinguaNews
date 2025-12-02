using LinguaNews.Models;
using LinguaNews.Options;
using Microsoft.Extensions.Options;
using System.Web;

namespace LinguaNews.Services
{
    public interface INewsDataIngestService
    {
        Task<IReadOnlyList<NewsDataArticle>> GetArticlesAsync(
            string? q,
            string language,
            CancellationToken ct = default);
    }

    public class NewsDataIngestService : INewsDataIngestService
    {
        private readonly HttpClient _httpClient;
        private readonly NewsDataOptions _options;

        public NewsDataIngestService(HttpClient httpClient, IOptions<NewsDataOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<IReadOnlyList<NewsDataArticle>> GetArticlesAsync(
            string? query,
            string language,
            CancellationToken ct = default)
        {
            var builder = new UriBuilder(_options.BaseUrl);
            var q = HttpUtility.ParseQueryString(builder.Query);

            q["apikey"] = _options.ApiKey;

            // Use the passed-in query, or pass blank q if empty
            if (!string.IsNullOrWhiteSpace(query))
            {
                q["q"] = query;
            }

            // Use the passed-in language (User selection), or default to Options ("en") if null
            if (!string.IsNullOrWhiteSpace(language))
            {
                q["language"] = language.ToLowerInvariant();
            }

            q["size"] = _options.PageSize.ToString();

            // Read only news from the last 7 days << API supports this, but we skip filtering and use "latest" API sorting instead
            //q["from_date"] = DateTime.UtcNow.AddDays(-7).ToString("yyyy-MM-dd");
            //q["to_date"] = today.ToString("yyyy-MM-dd");

            builder.Query = q.ToString();

            using var resp = await _httpClient.GetAsync(builder.Uri, ct);
            resp.EnsureSuccessStatusCode();

            var apiResponse = await resp.Content.ReadFromJsonAsync<NewsDataResponse>(cancellationToken: ct);

            // Return the raw list (or empty list if null)
            return apiResponse?.Results ?? new List<NewsDataArticle>();

        }
    }
}

