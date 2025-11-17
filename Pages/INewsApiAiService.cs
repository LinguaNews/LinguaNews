public interface INewsApiAiService
{
    Task<IReadOnlyList<NewsAiArticle>> GetArticlesAsync(
        string? query = null,
        CancellationToken ct = default);
}

using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using System.Web;

public class NewsApiAiService : INewsApiAiService
{
    private readonly HttpClient _httpClient;
    private readonly NewsApiAiOptions _options;

    public NewsApiAiService(HttpClient httpClient, IOptions<NewsApiAiOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<IReadOnlyList<NewsAiArticle>> GetArticlesAsync(string? query = null, CancellationToken ct = default)
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

        var data = await resp.Content.ReadFromJsonAsync<NewsAiResponse>(cancellationToken: ct)
                   ?? throw new InvalidOperationException("Failed to deserialize NewsAPI.ai response.");

        return data.Articles;
    }
}

