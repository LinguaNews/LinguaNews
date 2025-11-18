using LinguaNews.Data;
using LinguaNews.Models;
using Microsoft.EntityFrameworkCore;

namespace LinguaNews.Services	// added namespace as needed
{
	public class SmartTranslationService : ITranslationService
	{
		private readonly LinguaNewsDbContext _db;

		public SmartTranslationService(LinguaNewsDbContext db)
		{
			_db = db;
		}

		public async Task<string> TranslateAsync(string text, string targetLanguageCode)
		{
			string cleanText = text.Trim();

			// CHECK LOCAL CACHE FIRST (The "1000 Common Words" strategy)
			// We look for a match in the database to avoid API calls
			// We assume the cache in DB is stored in lowercase for easier matching
			var cachedWord = await _db.CommonWords
			    .FirstOrDefaultAsync(w =>
				   w.OriginalWord.ToLower() == cleanText.ToLower() &&
				   w.LanguageCode == targetLanguageCode);

			if (cachedWord != null)
			{
				return cachedWord.Translation; // Found - return immediately!
			}

			// FALLBACK TO API (Mocked for now)
			// In a real app, this is where you'd call DeepL or relavent API later
			await Task.Delay(200); // Simulate network latency
			return $"(API): {text}";
		}
	}
}