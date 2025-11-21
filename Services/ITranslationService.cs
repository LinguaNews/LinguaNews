namespace LinguaNews.Services
{
	// Mock for a Translation API (like DeepL)
	public interface ITranslationService
	{
		Task<string> TranslateAsync(string text, string targetLanguageCode);
	}
}
