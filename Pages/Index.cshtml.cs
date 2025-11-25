using LinguaNews.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Web;

namespace LinguaNews.Pages
{
	public class IndexModel : PageModel
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly ILogger<IndexModel> _logger;
		private readonly IConfiguration _config;

		public IndexModel(
		    IHttpClientFactory httpClientFactory,
		    ILogger<IndexModel> logger,
		    IConfiguration config)
		{
			_httpClientFactory = httpClientFactory;
			_logger = logger;
			_config = config;
		}

		[BindProperty(SupportsGet = true)]
		public string? SearchTerm { get; set; }

		[BindProperty(SupportsGet = true)]
		public string Language { get; set; } = "EN";

		public List<ArticleViewModel> Articles { get; set; } = new();

		public string? ErrorMessage { get; set; }

		public async Task OnGetAsync()
		{
			var client = _httpClientFactory.CreateClient();

			var apiKey = _config["NewsData:ApiKey"];
			var apiBaseUrl = _config["NewsData:BaseUrl"];
			var lastDaysConfig = _config["NewsData:LastHowManyDays"];

			if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiBaseUrl))
			{
				_logger.LogError("NewsData API configuration is incomplete");
				ErrorMessage = "Configuration error. Please check API settings.";
				return;
			}

			if (!int.TryParse(lastDaysConfig, out int daysBack))
			{
				daysBack = 7;
				_logger.LogWarning("Invalid 'LastHowManyDays' config. Using default: 7");
			}

			var today = DateTime.UtcNow.Date;
			var fromDate = today.AddDays(-daysBack);

			var query = HttpUtility.ParseQueryString(string.Empty);
			query["apikey"] = apiKey;
			query["language"] = Language;
			query["from_date"] = fromDate.ToString("yyyy-MM-dd");
			query["to_date"] = today.ToString("yyyy-MM-dd");
			query["q"] = !string.IsNullOrWhiteSpace(SearchTerm)
			    ? SearchTerm
			    : "language learning";

			var builder = new UriBuilder(apiBaseUrl) { Query = query.ToString() };
			string apiUrl = builder.ToString();

			try
			{
				var response = await client.GetAsync(apiUrl);

				if (!response.IsSuccessStatusCode)
				{
					_logger.LogWarning(
					    "NewsData API returned {StatusCode}: {Reason}",
					    response.StatusCode,
					    response.ReasonPhrase);
					ErrorMessage = "Unable to load articles. Please try again later.";
					return;
				}

				var apiResponse = await response.Content.ReadFromJsonAsync<NewsDataResponse>();

				if (apiResponse?.Results == null || apiResponse.Status != "success")
				{
					_logger.LogWarning("API response invalid or empty");
					ErrorMessage = "No articles available at this time.";
					return;
				}

				Articles = apiResponse.Results
				    .Where(item => !string.IsNullOrEmpty(item.Title)
							 && !string.IsNullOrEmpty(item.Description))
				    .Select(item => new ArticleViewModel
				    {
					    Title = item.Title ?? "No Title",
					    Description = item.Description ?? "No Description",
					    Url = item.Link ?? string.Empty,
					    UrlToImage = item.ImageUrl ?? string.Empty,
					    SourceName = item.SourceId ?? "Unknown Source"
				    })
				    .ToList();
			}
			catch (HttpRequestException ex)
			{
				_logger.LogError(ex, "Network error fetching articles from NewsData.io");
				ErrorMessage = "Network error. Please check your connection.";
			}
			catch (JsonException ex)
			{
				_logger.LogError(ex, "Failed to parse JSON response from NewsData.io");
				ErrorMessage = "Data format error. Please contact support.";
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error fetching articles");
				ErrorMessage = "An unexpected error occurred. Please try again.";
			}
		}
	}
}