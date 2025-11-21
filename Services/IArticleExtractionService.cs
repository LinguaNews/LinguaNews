namespace LinguaNews.Services
{
	// Mock for an Article Extraction API (like Diffbot)
	public interface IArticleExtractionService
	{
		Task<(string Title, string Text)> ExtractAsync(string url);
	}
}
