using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using System.Web;
using LinguaNews.Options;

namespace LinguaNews.Services
{
    public interface INewsAPIAIService
    {
        Task<IReadOnlyList<ArticleData>>GetArticlesAsync(string? q, CancellationToken ct);
    }

    public class NewsApiResponse
    {
        public string Status { get; set; } = string.Empty;
        public int TotalResults { get; set; }
        public List<ArticleData> Articles { get; set; } = new();
    }

    public class NewsApiAiService : INewsAPIAIService
	{
        private readonly HttpClient _httpClient;
        private readonly NewsApiOptions _options;

        public NewsApiAiService(HttpClient httpClient, IOptions<NewsApiOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<IReadOnlyList<ArticleData>> GetArticlesAsync(string? query = null, CancellationToken ct = default)
        {
            var builder = new UriBuilder(_options.BaseUrl);
            var q = HttpUtility.ParseQueryString(builder.Query);

            q["apiKey"] = _options.ApiKey;
            if (!string.IsNullOrWhiteSpace(query))
                q["query"] = query;
            q["pageSize"] = _options.PageSize.ToString();
            q["language"] = _options.Language;

            builder.Query = q.ToString();

            using var resp = await _httpClient.GetAsync(builder.Uri, ct);
            resp.EnsureSuccessStatusCode();

            NewsApiResponse data = await resp.Content.ReadFromJsonAsync<NewsApiResponse>(cancellationToken: ct)
                       ?? throw new InvalidOperationException("Failed to deserialize NewsAPI.ai response.");

            return data.Articles;
        }

        Task<IReadOnlyList<ArticleData>> INewsAPIAIService.GetArticlesAsync(string? query, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}

