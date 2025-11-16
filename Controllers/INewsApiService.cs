using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using System.Web;
using LinguaNews;
using LinguaNews.Controllers;
public interface INewsApiService
{
    Task<IReadOnlyList<ArticleData>> GetArticlesAsync(
        string? query = null,
        CancellationToken ct = default);
}
public class NewsApiAiService : INewsApiService
{
    private readonly HttpClient _httpClient;
    private readonly NewsApiOptions _TargetLanguage;

    public NewsApiAiService(HttpClient httpClient, IOptions<NewsApiOptions> options)
    {
        _httpClient = httpClient;
        options = (IOptions<NewsApiOptions>)options.Value;
    }

    /*public async Task<IReadOnlyList<ArticleData>> GetArticlesAsync(string? query = null, CancellationToken ct = default)
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
    }*/

    Task<IReadOnlyList<ArticleData>> INewsApiService.GetArticlesAsync(string? query, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}

