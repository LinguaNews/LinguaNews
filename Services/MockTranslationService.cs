namespace LinguaNews.Services
{
	public class MockTranslationService : ITranslationService
	{
		public async Task<string> TranslateAsync(string text, string targetLanguageCode)
		{
			// Simulate API call delay
			await Task.Delay(300);
			return $"({targetLanguageCode} Translation): Lorem ipsum dolor sit amet... (This is the translated text from the translation API)";
		}
	}
}
