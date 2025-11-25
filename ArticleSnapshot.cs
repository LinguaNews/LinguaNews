using LinguaNews.Models.LinguaNews;
using System.ComponentModel.DataAnnotations;

namespace LinguaNews.Models
{
    public class ArticleSnapshot
    {
        public int Id { get; set; }
        [Required]
        public string OriginalUrl { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        [Required]
        public string OriginalText { get; set; } = string.Empty;
        public string TargetLanguage { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        //DisplayTranslation needs to be adjusted. This is a placeholder so that it compiles.
        public string DisplayTranslation { get; set; } = string.Empty;
        public CurrentArticles CurrentArticle { get; set; } = new CurrentArticles();
        public DateTime FetchedAt { get; set; } = DateTime.UtcNow;
        public List<Translation> Translations { get; set; } = new List<Translation>();
    }
    /* * These are mock services. In a real app, you would inject
     * real services that use HttpClient to call your APIs.
     */
    namespace LinguaNews
    {
        // Mock for an Article Extraction API (like Diffbot)
        public interface IArticleExtractionService
        {
            Task<(string Title, string Text)> ExtractAsync(string url);
        }

        public class MockArticleExtractionService : IArticleExtractionService
        {
            public async Task<(string Title, string Text)> ExtractAsync(string url)
            {
                // Simulate API call delay
                await Task.Delay(500);
                return (
                    "This is the Fetched Article Title",
                    "Lorem ipsum dolor sit amet, consectetur adipiscing elit. ... (This is the full article text that was fetched from the extraction API) ... sed do eiusmod tempor incididunt ut labore et dolore magna aliqua."
                );
            }
        }

        // Mock for a Translation API (like DeepL)
        public interface ITranslationService
        {
            Task<string> TranslateAsync(string text, string targetLanguageCode);
        }

        public class MockTranslationService : ITranslationService
        {
            public async Task<string> TranslateAsync(string text, string targetLanguageCode)
            {
                // Simulate API call delay
                await Task.Delay(300);
                return $"({targetLanguageCode} Translation): Lorem ipsum dolor sit amet... (This is the translated text from the translation API)";
            }
        }

        public class CurrentArticles
        {
            public string Title { get; set; } = string.Empty;
            public string OriginalUrl { get; set; } = string.Empty;
            public string OriginalText { get; set; } = string.Empty;
        }
    }
}


