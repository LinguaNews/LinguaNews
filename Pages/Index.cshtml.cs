using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Web;
using LinguaNews.Models.LinguaNews;

namespace LinguaNews.Pages
{
    public class IndexModel : PageModel
    {
        // Use IHttpClientFactory (injected) instead of a static client
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<IndexModel> _logger;

        // Use the API key and dates from your file
        private const string ApiKey = "pub_731b19e405ac490a9761d29863e3e748";
        private const string ApiBaseUrl = "https://newsdata.io/api/1/archive";

        // Inject services in the constructor
        public IndexModel(IHttpClientFactory httpClientFactory, ILogger<IndexModel> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        // This binds to the "name='SearchTerm'" input in the .cshtml file.
        // SupportsGet = true is required for the form's "get" method.
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        // This binds to the "name='Language'" dropdown.
        // It defaults to "en" (English) if nothing is selected.
        [BindProperty(SupportsGet = true)]
        public string Language { get; set; } = "en";

        // It holds the final list of articles for the view to display.
        public List<ArticleViewModel> Articles { get; set; } = [];

        // A user-friendly error message to display in the UI when the API fails.
        public string? ErrorMessage { get; set; }

        // Indicates if data is currently loading (for showing a spinner in the UI).
        public bool IsLoading { get; private set; }

        // Change OnGet to be asynchronous
        public async Task OnGetAsync()
        {
            IsLoading = true;

            // ✅ Normalize and validate search input before building API query
            SearchTerm = SearchTerm?.Trim();

            if (string.IsNullOrWhiteSpace(SearchTerm))
            {
                // Default search keyword to prevent empty API calls
                SearchTerm = "language learning";
            }

            if (SearchTerm != null && SearchTerm.Length > 50)
            {
                // Limit excessively long queries that can cause 400 errors
                SearchTerm = SearchTerm.Substring(0, 50);
            }

            try
            {
                var client = _httpClientFactory.CreateClient();

                // Build the query string dynamically
                var query = HttpUtility.ParseQueryString(string.Empty);
                query["apikey"] = ApiKey;
                query["language"] = Language;
                query["from_date"] = "2025-11-09"; // Date from your file
                query["to_date"] = "2025-11-16";   // Date from your file
                query["q"] = SearchTerm;

                var builder = new UriBuilder(ApiBaseUrl) { Query = query.ToString() };
                string apiUrl = builder.ToString();

                // Use RequestAborted so the call cancels if the user navigates away.
                var response = await client.GetAsync(apiUrl, HttpContext.RequestAborted);

                // Handle non-success status codes explicitly.
                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    {
                        ErrorMessage = "API rate limit reached. Please try again later.";
                        _logger.LogWarning("NewsData.io API rate limit hit (HTTP 429).");
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                    {
                        ErrorMessage = "Access to the news service was denied. Please verify the API key.";
                        _logger.LogWarning("NewsData.io API returned 403 Forbidden.");
                    }
                    else
                    {
                        ErrorMessage = "Unable to load articles right now. Please try again later.";
                        _logger.LogWarning("NewsData.io API call failed with status code {StatusCode}", response.StatusCode);
                    }

                    return;
                }

                // Deserialize the JSON Response
                var apiResponse = await response.Content.ReadFromJsonAsync<NewsDataResponse>();

                if (apiResponse == null || apiResponse.Status != "success")
                {
                    ErrorMessage = "Received an unexpected response from the news service.";
                    _logger.LogWarning("NewsData.io returned null or non-success status: {Status}", apiResponse?.Status);
                    return;
                }

                if (apiResponse.Results == null || apiResponse.Results.Count == 0)
                {
                    ErrorMessage = $"No articles available in {Language} for the selected dates.";
                    Articles = [];
                    return;
                }

                // Map the API data to our ArticleViewModel
                Articles = apiResponse.Results
                    // Filter out articles with no title/description
                    .Where(item => !string.IsNullOrEmpty(item.Title) && !string.IsNullOrEmpty(item.Description))
                    .Select(static item => new ArticleViewModel
                    {
                        Title = item.Title ?? "No Title",
                        Description = item.Description ?? "No Description",
                        Url = item.Link ?? string.Empty,              // Uses the "link" property from the API
                        UrlToImage = item.ImageUrl ?? string.Empty,  // Uses the "image_url" property
                        SourceName = item.SourceId ?? "Unknown Source" // Uses "source_id"
                    })
                    .ToList();
            }
            catch (OperationCanceledException)
            {
                // Request was cancelled (e.g., user navigated away). No user message needed.
                _logger.LogInformation("NewsData.io request was cancelled.");
            }
            catch (Exception ex)
            {
                ErrorMessage = "An unexpected error occurred while loading articles.";
                _logger.LogError(ex, "Error fetching articles from NewsData.io");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }

    // It contains only the data we need to display on the page.
    public class ArticleViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string UrlToImage { get; set; } = string.Empty;
        public string SourceName { get; set; } = string.Empty;
    }

    // This wrapper class matches the overall JSON structure from NewsData.io
    // { "status": "success", "totalResults": 123, "results": [...] }
    public class NewsDataResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("totalResults")]
        public int TotalResults { get; set; }

        [JsonPropertyName("results")]
        public List<NewsDataArticle> Results { get; set; } = [];
    }

    // This class represents a single article from the API.
    // We use JsonPropertyName to map snake_case (like "image_url") to PascalCase.
    public class NewsDataArticle
    {
        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("link")]
        public string? Link { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("image_url")]
        public string? ImageUrl { get; set; }

        [JsonPropertyName("source_id")]
        public string? SourceId { get; set; }
    }
}
