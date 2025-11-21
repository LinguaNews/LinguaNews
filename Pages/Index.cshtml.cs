using LinguaNews.Models;
using LinguaNews.Models.LinguaNews;
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
        private readonly IConfiguration _config;
        

		private const string ApiKey = "pub_731b19e405ac490a9761d29863e3e748";
		private const string ApiBaseUrl = "https://newsdata.io/api/1/archive";

		public IndexModel(IHttpClientFactory httpClientFactory, ILogger<IndexModel> logger)
		{
			_httpClientFactory = httpClientFactory;
			_logger = logger;
            _config = config;
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

            var apiKey = _config["NewsData:ApiKey"];
            var apiBaseUrl = _config["NewsData:BaseUrl"];
			var lastDays = _config["NewsData:LastHowManyDays"];

            var today = DateTime.UtcNow.Date;
            var fromDate = today.AddDays(int.Parse(lastDays));

            // Build the query string dynamically
            var query = HttpUtility.ParseQueryString(string.Empty);
			query["apikey"] = apiKey;
			query["language"] = Language;
			query["from_date"] = fromDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
			query["to_date"] = today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

            // Add the search term IF the user provided one, otherwise use a default
            query["q"] = !string.IsNullOrWhiteSpace(SearchTerm)
			    ? SearchTerm
			    : "language learning"; // Default search if none provided

			var builder = new UriBuilder(apiBaseUrl) { Query = query.ToString() };
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
