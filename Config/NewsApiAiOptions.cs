namespace LinguaNews.Options
{
	public class NewsApiAiOptions
	{
		public string BaseUrl { get; set; } = string.Empty;
		public string ApiKey { get; set; } = string.Empty;
		public int PageSize { get; set; } = 20;
		public string Language { get; set; } = "en";
	}
}
