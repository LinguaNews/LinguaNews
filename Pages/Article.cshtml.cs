using LinguaNews.Data; // <-- Assumes LinguaNewsDbContext is in this namespace
using LinguaNews.Models;
using LinguaNews.Models.LinguaNews;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace LinguaNews.Pages
{
    public class ArticleModel : PageModel
    {
        // --- CHANGE 1: The type of your DbContext field ---
        private readonly LinguaNewsDbContext _db;
        private readonly IArticleExtractionService _extractionService;
        private readonly ITranslationService _translationService;

        // --- CHANGE 2: The type in the constructor's parameter ---
        public ArticleModel(
            LinguaNewsDbContext db, // <-- Changed from ApplicationDbContext
            IArticleExtractionService extractionService,
            ITranslationService translationService)
        {
            _db = db; // <-- Assign the correct DbContext
            _extractionService = extractionService;
            _translationService = translationService;
        }

        // --- Properties to hold data for the View (No changes) ---

        [BindProperty(SupportsGet = true)]
        public new string Url { get; set; } = string.Empty;

        [BindProperty]
        public string TargetLanguage { get; set; } = "ES";

        public ArticleSnapshot? CurrentArticle { get; set; }

        public string? DisplayTranslation { get; set; }


        // --- Page Handlers (No changes needed) ---
        // The logic was already correct. It uses the _db variable,
        // which is now correctly of type LinguaNewsDbContext.

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrWhiteSpace(Url))
            {
                return NotFound("No article URL provided.");
            }

            // 1. Try to find the article in our database
            var snapshot = await _db.ArticleSnapshots
                .Include(s => s.Translations) // <-- Correctly includes the Translation model
                .FirstOrDefaultAsync(a => a.OriginalUrl == Url);

            if (snapshot == null)
            {
                // 2. Fetch it
                var (title, text) = await _extractionService.ExtractAsync(Url);

                if (string.IsNullOrWhiteSpace(text))
                {
                    return NotFound("Could not extract article text.");
                }

                // Create the new snapshot
                snapshot = new ArticleSnapshot
                {
                    OriginalUrl = Url,
                    Title = title,
                    OriginalText = text,
                    FetchedAt = DateTime.UtcNow
                };

                // 3. Save to database
                _db.ArticleSnapshots.Add(snapshot);
                await _db.SaveChangesAsync();
            }

            // 4. Set the property for the view
            CurrentArticle = snapshot;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Url))
            {
                return NotFound();
            }

            // 1. Load the article
            var snapshot = await _db.ArticleSnapshots
                //.Include(s => s.Translations)//
                .FirstOrDefaultAsync(a => a.OriginalUrl == Url);

            if (snapshot == null)
            {
                return RedirectToPage("/Index");
            }

            CurrentArticle = snapshot;

            // 2. Check for existing translation
            var existingTranslation = snapshot.Translations
                .FirstOrDefault(t => t.LanguageCode == TargetLanguage);

            if (existingTranslation != null)
            {
                // 3. Cache Hit
                DisplayTranslation = existingTranslation.TranslatedText;
            }
            else
            {
                // 4. Cache Miss! Call API
                var translatedText = await _translationService.TranslateAsync(
                    snapshot.OriginalText,
                    TargetLanguage
                );

                // 5. Create and save the new Translation model
                var newTranslation = new Translation
                {
                    LanguageCode = TargetLanguage,
                    TranslatedText = translatedText,
                    ArticleSnapshotId = snapshot.Id
                };

                _db.Translations.Add(newTranslation); // <-- Correctly adds to the Translations DbSet
                await _db.SaveChangesAsync();

                // 6. Display the new translation
                DisplayTranslation = translatedText;
            }

            return Page();
        }
    }
}
