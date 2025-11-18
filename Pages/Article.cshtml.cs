using LinguaNews.Data;
using LinguaNews.Models;
using LinguaNews.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LinguaNews.Pages
{
	public class ArticleModel : PageModel
	{
		private readonly LinguaNewsDbContext _db;
		private readonly IArticleExtractionService _extractionService;
		private readonly ITranslationService _translationService;

		// Constructor
		public ArticleModel(
		    LinguaNewsDbContext db,
		    IArticleExtractionService extractionService,
		    ITranslationService translationService)
		{
			_db = db;
			_extractionService = extractionService;
			_translationService = translationService;
		}

		// Properties
		[BindProperty(SupportsGet = true)]
		public new string Url { get; set; } = string.Empty;

		[BindProperty]
		public string TargetLanguage { get; set; } = "ES";

		public ArticleSnapshot? DisplayArticle { get; set; }
		public string? DisplayTranslation { get; set; }

		// Handler: Load the page
		public async Task<IActionResult> OnGetAsync()
		{
			if (string.IsNullOrWhiteSpace(Url)) return NotFound("No article URL provided.");

			// 1. Check DB
			var snapshot = await _db.ArticleSnapshots
			    .Include(s => s.Translations)
			    .FirstOrDefaultAsync(a => a.OriginalUrl == Url);

			// 2. If missing, fetch from web
			if (snapshot == null)
			{
				var (title, text) = await _extractionService.ExtractAsync(Url);

				if (string.IsNullOrWhiteSpace(text)) return NotFound("Could not extract article text.");

				snapshot = new ArticleSnapshot
				{
					OriginalUrl = Url,
					Title = title,
					OriginalText = text,
					FetchedAt = DateTime.UtcNow
				};

				_db.ArticleSnapshots.Add(snapshot);
				await _db.SaveChangesAsync();
			}

			DisplayArticle = snapshot;
			return Page();
		}

		// Handler: Full Page Translation
		public async Task<IActionResult> OnPostAsync()
		{
			if (string.IsNullOrWhiteSpace(Url)) return NotFound();

			var snapshot = await _db.ArticleSnapshots
			    .Include(s => s.Translations)
			    .FirstOrDefaultAsync(a => a.OriginalUrl == Url);

			if (snapshot == null) return RedirectToPage("/Index");

			DisplayArticle = snapshot;

			// Check Cache
			var existingTranslation = snapshot.Translations
			    .FirstOrDefault(t => t.LanguageCode == TargetLanguage);

			if (existingTranslation != null)
			{
				DisplayTranslation = existingTranslation.TranslatedText;
			}
			else
			{
				// Call Service
				var translatedText = await _translationService.TranslateAsync(
				    snapshot.OriginalText,
				    TargetLanguage
				);

				// Save Result
				var newTranslation = new Translation
				{
					LanguageCode = TargetLanguage,
					TranslatedText = translatedText,
					ArticleSnapshotId = snapshot.Id
				};

				_db.Translations.Add(newTranslation);
				await _db.SaveChangesAsync();

				DisplayTranslation = translatedText;
			}

			return Page();
		}

		// Handler: Single Word Lookup (AJAX)
		public async Task<IActionResult> OnGetWordLookupAsync(string word, string language)
		{
			if (string.IsNullOrWhiteSpace(word) || string.IsNullOrWhiteSpace(language))
			{
				return BadRequest("Word and language are required.");
			}

			var translatedWord = await _translationService.TranslateAsync(word.Trim(), language);

			return new JsonResult(new { translation = translatedWord });
		}
	}
}