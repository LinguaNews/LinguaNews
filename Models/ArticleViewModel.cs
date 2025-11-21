namespace LinguaNews.Models
{
	// A lightweight model used to display articles on the main feed page (Index.cshtml).
	public class ArticleViewModel
	{
		public string Title { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public string Url { get; set; } = string.Empty;
		public string UrlToImage { get; set; } = string.Empty;
		public string SourceName { get; set; } = string.Empty;
	}
}