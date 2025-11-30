using LinguaNews.Models;
using LinguaNews.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using LinguaNews.Data; 
using System.Web;

namespace LinguaNews.Pages
{
	public class IndexModel : PageModel
	{
        private readonly INewsDataIngestService _newsService;
        private readonly LinguaNewsDbContext _db;
        private readonly ILogger<IndexModel> _logger;
		
        public IndexModel(
            INewsDataIngestService newsService,
            LinguaNewsDbContext db,
		    ILogger<IndexModel> logger)
        {
            _newsService = newsService;
            _db = db;
            _logger = logger;
		}

		[BindProperty(SupportsGet = true, Name = "q")]
		public string? SearchTerm { get; set; }

		[BindProperty(SupportsGet = true)]
		public string Language { get; set; } = "EN";

		public List<ArticleViewModel> Articles { get; set; } = new();
        public List<ArticleSnapshot> SavedArticles { get; set; } = new();
        public string? ErrorMessage { get; set; }

		public async Task OnGetAsync()
		{
            //  LOAD HISTORY FROM DB
            try
            {
                SavedArticles = await _db.ArticleSnapshots
                    .OrderByDescending(a => a.FetchedAt) // Newest first
                    .Take(4) // Limit to 4 to keep UI clean
                    .ToListAsync();
            }
            catch
            {
                // If DB fails (migration issue), ignore it so page still loads
            }
            if (string.IsNullOrWhiteSpace(SearchTerm))
            {
                Articles = new List<ArticleViewModel>();
                return;
            }
				try
				 {
					 // We pass the SearchTerm and Language directly from the form properties
					 var rawArticles = await _newsService.GetArticlesAsync(SearchTerm, Language, CancellationToken.None);

					 if (rawArticles == null || !rawArticles.Any())
                {
						ErrorMessage = "No articles found. Try adjusting your search.";
							return;
                }

                // Map Data -> ViewModel (The "Controller" Logic)
                Articles = rawArticles
                    .Where(a => !string.IsNullOrEmpty(a.Title)) // Basic filter
                    .Select(a => new ArticleViewModel
                    {
                        Title = a.Title ?? "No Title",

                        // Handle the description fallback if content is not available
                        Description = !string.IsNullOrWhiteSpace(a.Description)
                                      ? a.Description
                                      : "No description available.",
						Url = a.Link ?? "#",

                        // Use a placeholder if image is missing (place in images folder)
                        UrlToImage = !string.IsNullOrWhiteSpace(a.ImageUrl)
                                     ? a.ImageUrl
                                     : "https://www.google.com/url?sa=E&source=gmail&q=via.placeholder.com",

                        SourceName = a.SourceId ?? "Unknown"
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