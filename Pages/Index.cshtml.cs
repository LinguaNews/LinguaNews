using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Web;
using LinguaNews.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LinguaNews.Pages
{
	public class IndexModel : PageModel
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly ILogger<IndexModel> _logger;

		private const string ApiKey = "pub_731b19e405ac490a9761d29863e3e748";
		private const string ApiBaseUrl = "https://newsdata.io/api/1/archive";

		public IndexModel(IHttpClientFactory httpClientFactory, ILogger<IndexModel> logger)
		{
			_httpClientFactory = httpClientFactory;
			_logger = logger;
		}

		[BindProperty(SupportsGet = true)]
		public string? SearchTerm { get; set; }

		[BindProperty(SupportsGet = true)]
		public string Language { get; set; } = "EN";

		public List<ArticleViewModel> Articles { get; set; } = [];

		public async Task OnGetAsync()
		{
			var currentLanguage = Language;

			var client = _httpClientFactory.CreateClient();
			var query = System.Web.HttpUtility.ParseQueryString(string.Empty);

			// Build Query
			query["apikey"] = ApiKey;
			query["language"] = Language;
			query["from_date"] = "2025-11-09";
			query["to_date"] = "2025-11-16";
			query["q"] = !string.IsNullOrWhiteSpace(SearchTerm) ? SearchTerm : "language learning";

			var builder = new UriBuilder(ApiBaseUrl) { Query = query.ToString() };
			string apiUrl = builder.ToString();

			try
			{
				var response = await client.GetAsync(apiUrl);

				if (response.IsSuccessStatusCode)
				{
					var apiResponse = await response.Content.ReadFromJsonAsync<NewsDataResponse>();

					if (apiResponse?.Results != null)
					{
						Articles = apiResponse.Results
						  .Where(item => !string.IsNullOrEmpty(item.Title) && !string.IsNullOrEmpty(item.Description))
						  .Select(static item => new ArticleViewModel
						  {
							  Title = item.Title ?? "No Title",
							  Description = item.Description ?? "No Description",
							  Url = item.Link ?? string.Empty,
							  UrlToImage = item.ImageUrl ?? string.Empty,
							  SourceName = item.SourceId ?? "Unknown Source"
						  })
						  .ToList();
					}
				}
				else
				{
					_logger.LogWarning("NewsData.io API call failed with status code {StatusCode}", response.StatusCode);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error fetching articles from NewsData.io");
			}
		}
	}
}