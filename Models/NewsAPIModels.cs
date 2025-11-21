using System.Text.Json.Serialization;

namespace LinguaNews.Models
{
	// This wrapper class matches the overall JSON structure from NewsData.io
	// It was previously embedded in Index.cshtml.cs
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
	// It was previously embedded in Index.cshtml.cs
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