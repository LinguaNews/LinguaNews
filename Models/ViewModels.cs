using System.Text.Json.Serialization;

namespace LinguaNews.Models
{
    // NOTE: This contains only the data we need to display on the INDEX page. DOMAIN MODELS used to populate ArticleSnapshot
    public class ArticleViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string UrlToImage { get; set; } = string.Empty;
        public string SourceName { get; set; } = string.Empty;
    }
}
