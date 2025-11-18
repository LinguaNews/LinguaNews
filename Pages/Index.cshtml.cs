using LinguaNews.Models.LinguaNews;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;

namespace LinguaNews.Pages
{
	public class IndexModel : PageModel
	{
		// Use IHttpClientFactory (injected) instead of a static client
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly ILogger<IndexModel> _logger;
        private readonly IConfiguration _config;
        

        // Inject services in the constructor, can be relocated to subpage later
        public IndexModel(IHttpClientFactory httpClientFactory, ILogger<IndexModel> logger, IConfiguration config)
		{
			_httpClientFactory = httpClientFactory;
			_logger = logger;
            _config = config;
        }

		// Add properties to bind to the search form

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


		// Change OnGet to be Asynchronous
		public async Task OnGetAsync()
		{
			// This replaces your LoadMockArticles()

			var client = _httpClientFactory.CreateClient();

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
				// Call the API
				var response = await client.GetAsync(apiUrl);

				if (response.IsSuccessStatusCode)
				{
					// Deserialize the JSON Response
					var apiResponse = await response.Content.ReadFromJsonAsync<NewsDataResponse>();

					if (apiResponse != null && apiResponse.Status == "success" && apiResponse.Results != null)
					{
						// Map the API data to our ArticleViewModel
						Articles = apiResponse.Results
						    // Filter out articles with no title/description
						    .Where(item => !string.IsNullOrEmpty(item.Title) && !string.IsNullOrEmpty(item.Description))
						    .Select(static item => new ArticleViewModel
						    {
							    Title = item.Title ?? "No Title",
							    Description = item.Description ?? "No Description",
							    Url = item.Link, // Uses the "link" property from the API
							    UrlToImage = item.ImageUrl, // Uses the "image_url" property
							    SourceName = item.SourceId ?? "Unknown Source" // Uses "source_id"
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

		// --- This helper method is no longer needed, as we're calling the real API ---
		/*
using microsoft.aspnetcore.mvc;
using microsoft.aspnetcore.mvc.razorpages;
using system.text.json;
using system.text.json.serialization;
using system.web;
using linguanews.models.linguanews;


namespace linguanews.pages
{
    public class indexmodel : pagemodel
    {
        static readonly httpclient client = new httpclient();
        // this property will hold our list of articles for the view to display
        [bindproperty(supportsget = true)]
        public list<articleviewmodel> articles { get; set; } = [];

        // this is where you will eventually call the newsapi.
        public void onget()
        {
            var loadarticledata = client.getasync("https://newsdata.io/api/1/archive?apikey=pub_731b19e405ac490a9761d29863e3e748&q=example&language=en&from_date=2025-11-09&to_date=2025-11-16");
            articles = loadmockarticles();
        }

        /// <summary>
        /// a private helper method to create placeholder data.
        /// later, you will replace this with a real api call.
        /// </summary>
        private list<articleviewmodel> loadmockarticles()
        {
            return new list<articleviewmodel>
            {
                new articleviewmodel
                {
                    title = "major breakthrough in ai language translation",
                    description = "a new deep-learning model has shown unprecedented accuracy in real-time translation, paving the way for tools just like linguanews.",
                    url = "https://example.com/article-1", // placeholder link
                    urltoimage = "https://via.placeholder.com/350x200.png?text=ai+translation", // placeholder image
                    sourcename = "tech news daily"
                },
                new articleviewmodel
                {
                    title = "global markets respond to new policies",
                    description = "stock markets around the world saw significant movement today after the announcement of new international trade agreements.",
                    url = "https://example.com/article-2",
                    urltoimage = "https://via.placeholder.com/350x200.png?text=global+markets",
                    sourcename = "world finance times"
                },
                new articleviewmodel
                {
                    title = "the rise of polyglot programming",
                    description = "developers are increasingly learning multiple programming languages, but what about spoken languages? we explore the trend.",
                    url = "https://example.com/article-3",
                    urltoimage = "https://via.placeholder.com/350x200.png?text=programming",
                    sourcename = "developer weekly"
                }
            };
        }
    }

    /// <summary>
    /// this is a "view model" representing a single article.
    /// it contains only the data we need to display on the page.
    /// you would typically populate this from the full model you get from newsapi.
    /// </summary>
    public class articleviewmodel
    {
        public string title { get; set; } = string.empty;
        public string description { get; set; } = string.empty;
        public string url { get; set; } = string.empty;
        public string urltoimage { get; set; } = string.empty;
        public string sourcename { get; set; } = string.empty;
    }
}
*/
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


	// Add classes to deserialize the NewsData.io JSON response
	// These match the fields in your ArticleData.cs but use System.Text.Json

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

	// This class represents a single article *from the API*.
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
