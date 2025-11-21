namespace LinguaNews.Services
{
	public class MockArticleExtractionService : IArticleExtractionService
	{
		public async Task<(string Title, string Text)> ExtractAsync(string url)
		{
			// Simulate API delay
			await Task.Delay(200);

			return (
			    "Mock Article Title",
			    "This is the text returned by the mock service. In a real app, this would be the scraped content from: " + url
			);
		}
	}
}