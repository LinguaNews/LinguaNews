using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LinguaNews.Pages
{
    public class IndexModel : PageModel
    {
        // This property will hold our list of articles for the view to display
        [BindProperty(SupportsGet = true)]
        public List<ArticleViewModel> Articles { get; set; } = new List<ArticleViewModel>();

        // This is where you will eventually call the NewsAPI.
        public void OnGet()
        {
            // For now, we load mock data as a placeholder.
            Articles = LoadMockArticles();
        }

        /// <summary>
        /// A private helper method to create placeholder data.
        /// Later, you will replace this with a real API call.
        /// </summary>
        private List<ArticleViewModel> LoadMockArticles()
        {
            return new List<ArticleViewModel>
            {
                new ArticleViewModel
                {
                    Title = "Major Breakthrough in AI Language Translation",
                    Description = "A new deep-learning model has shown unprecedented accuracy in real-time translation, paving the way for tools just like LinguaNews.",
                    Url = "https://example.com/article-1", // Placeholder link
                    UrlToImage = "https://via.placeholder.com/350x200.png?text=AI+Translation", // Placeholder image
                    SourceName = "Tech News Daily"
                },
                new ArticleViewModel
                {
                    Title = "Global Markets Respond to New Policies",
                    Description = "Stock markets around the world saw significant movement today after the announcement of new international trade agreements.",
                    Url = "https://example.com/article-2",
                    UrlToImage = "https://via.placeholder.com/350x200.png?text=Global+Markets",
                    SourceName = "World Finance Times"
                },
                new ArticleViewModel
                {
                    Title = "The Rise of Polyglot Programming",
                    Description = "Developers are increasingly learning multiple programming languages, but what about spoken languages? We explore the trend.",
                    Url = "https://example.com/article-3",
                    UrlToImage = "https://via.placeholder.com/350x200.png?text=Programming",
                    SourceName = "Developer Weekly"
                }
            };
        }
    }

    /// <summary>
    /// This is a "View Model" representing a single article.
    /// It contains only the data we need to display on the page.
    /// You would typically populate this from the full model you get from NewsAPI.
    /// </summary>
    public class ArticleViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string UrlToImage { get; set; } = string.Empty;
        public string SourceName { get; set; } = string.Empty;
    }
}
