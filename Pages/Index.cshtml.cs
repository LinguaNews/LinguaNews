using LinguaNews.Data; 
using LinguaNews.Models;
using LinguaNews.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace LinguaNews.Pages
{
	public class IndexModel : PageModel
	{
        private readonly INewsDataIngestService _newsService;
        private readonly LinguaNewsDbContext _db;
        private readonly IMemoryCache _cache;
        private readonly ILogger<IndexModel> _logger;
		
        public IndexModel(
            INewsDataIngestService newsService,
            LinguaNewsDbContext db,
            IMemoryCache cache,
		    ILogger<IndexModel> logger)
        {
            _newsService = newsService;
            _db = db;
            _cache = cache;
            _logger = logger;
		}

		[BindProperty(SupportsGet = true, Name = "q")]
		public string? SearchTerm { get; set; }

		[BindProperty(SupportsGet = true)]
		public string? Language { get; set; }

		public List<ArticleViewModel> Articles { get; set; } = new();
        public bool IsCachedResponse { get; set; } = false;
        public List<ArticleSnapshot> SavedArticles { get; set; } = new();
        public string? ErrorMessage { get; set; }

		public async Task OnGetAsync()
		{
            //  LOAD HISTORY FROM DB
            try
            {
                SavedArticles = await _db.ArticleSnapshots
                    .OrderByDescending(a => a.FetchedAt) // Newest first
                    .Take(6) // Limit to 6
                    .ToListAsync();
            }
            catch
            {
                // If DB fails (migration issue), ignore it so page still loads LOGGING CAN BE ADDED HERE
            }
            if (string.IsNullOrWhiteSpace(SearchTerm)) return;

            // [CACHE] 5. Define a Unique Key
            // e.g. "Search_bitcoin_es"
            // If we didn't include Language, searching Bitcoin in Spanish would show English results!
            string cacheKey = $"Search_{SearchTerm.ToLower()}_{Language?.ToLower() ?? "all"}";

            // ?? BREAKPOINT HERE: Click the left margin on the line below.
            // If you step OVER (F10) and it skips the 'if' block, Caching is WORKING.
            if (!_cache.TryGetValue(cacheKey, out List<ArticleViewModel>? cachedResults))
            {
                // --- CACHE MISS (We have to pay 1 Credit) ---
                IsCachedResponse = false;

                try
                {
                    var rawArticles = await _newsService.GetArticlesAsync(
                        SearchTerm,
                        Language ?? "en",
                        CancellationToken.None);

                    if (rawArticles == null || !rawArticles.Any())
                    {
                        ErrorMessage = "No articles found. Try adjusting your search.";
                        return;
                    }

                    Articles = rawArticles
                        .Where(a => !string.IsNullOrEmpty(a.Title))
                        .Select(a => new ArticleViewModel
                        {
                            Title = a.Title ?? "No Title",
                            Description = a.Description ?? "No Description",
                            Url = a.Link ?? string.Empty,
                            UrlToImage = !string.IsNullOrWhiteSpace(a.ImageUrl) ? a.ImageUrl:"/LingaNews.png", 
                            SourceName = a.SourceId ?? "Unknown",
                        })
                        .ToList();

                    // [CACHE] 6. Save data to memory
                    // "Keep this list in RAM for 15 minutes"
                    var cacheOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(15));

                    _cache.Set(cacheKey, Articles, cacheOptions);
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
            else
            {
                // --- CACHE HIT (Free!) ---
                IsCachedResponse = true;

                // [CACHE] 7. Use the data we found in memory
                Articles = cachedResults ?? new List<ArticleViewModel>();
            }
        }
    }
}
