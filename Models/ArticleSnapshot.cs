using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

		public string DisplayTranslation { get; set; } = string.Empty;
		[NotMapped]
		public DisplayArticles DisplayArticle { get; set; } = new DisplayArticles();

		public DateTime FetchedAt { get; set; } = DateTime.UtcNow;

		// Relationship to Translations
		public List<Translation> Translations { get; set; } = new List<Translation>();
	}

	// Kept simple and in the same namespace
	public class DisplayArticles
	{
		public string Title { get; set; } = string.Empty;
		public string OriginalUrl { get; set; } = string.Empty;
		public string OriginalText { get; set; } = string.Empty;
	}
}