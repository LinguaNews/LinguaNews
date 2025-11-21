using System.Text.Json.Serialization;

namespace LinguaNews.Models
{
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
