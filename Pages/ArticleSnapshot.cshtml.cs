using LinguaNews.Data;
using LinguaNews.Models;
using LinguaNews.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LinguaNews.Pages
{
    public class ArticleSnapshotModel : PageModel
    {
        private readonly LinguaNewsDbContext _db;
        private readonly INewsDataIngestService _ingestService;

        public ArticleSnapshotModel(LinguaNewsDbContext db, INewsDataIngestService ingestService)
        {
            _db = db;
            _ingestService = ingestService;
        }

        // --- INPUTS FROM URL ---
        [BindProperty(SupportsGet = true)]
        public string ArticleUrl { get; set; } = string.Empty;

        // STORAGE EFFICIENCY Capture these from the link on the Index page
        [BindProperty(SupportsGet = true)]
        public string? IncomingTitle { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? IncomingDescription { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? IncomingSource { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string? IncomingImage { get; set; }

        // --- STATE ---
        [BindProperty]
        public string TargetLanguage { get; set; } = "ES";

        public ArticleSnapshot? Snapshot { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrWhiteSpace(ArticleUrl)) return RedirectToPage("/Index");

            // Check if we already have this article saved
            var existingSnapshot = await _db.ArticleSnapshots
                .Include(a => a.Translations)
                .FirstOrDefaultAsync(a => a.ArticleUrl == ArticleUrl);

            if (existingSnapshot == null)
            {
                // Prefer the data passed from Index page!
                // This solves the "Free Tier" problem because we use the data we already found. [NOTE: AI GENERATED CODE]
                
                string finalTitle = IncomingTitle ?? "Unknown Title";
                string finalContent = string.Empty;
                string finalDesc = IncomingDescription ?? string.Empty;

                // Only call the scraper if we have absolutely no data, THIS IS A MOCK OF AN ACTUAL SCRAPER CALL
                if (string.IsNullOrWhiteSpace(IncomingDescription))
                {
                     var extracted = await _ingestService.ExtractAsync(ArticleUrl);
                     finalTitle = extracted.Title;
                     finalContent = extracted.Content;
                } 

                existingSnapshot = new ArticleSnapshot
                {
                    ArticleUrl = ArticleUrl,
                    Title = finalTitle,
                    Description = finalDesc,       // Save the description
                    Content = finalContent,        // Might be empty due to free tier
                    SourceName = IncomingSource ?? "Unknown",
                    ImageUrl = IncomingImage ?? string.Empty,
                    FetchedAt = DateTime.UtcNow
                }; // [END AI GENERATED CODE]

                _db.ArticleSnapshots.Add(existingSnapshot);
                await _db.SaveChangesAsync();
            }

            Snapshot = existingSnapshot;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(ArticleUrl)) return RedirectToPage("/Index");

            var snapshot = await _db.ArticleSnapshots
                .Include(a => a.Translations)
                .FirstOrDefaultAsync(a => a.ArticleUrl == ArticleUrl);

            if (snapshot == null) return RedirectToPage("/Index");

            Snapshot = snapshot;

            // Check Cache
            var cachedTranslation = snapshot.Translations
                .FirstOrDefault(t => t.LanguageCode == TargetLanguage);

            if (cachedTranslation != null)
            {
                Snapshot.DisplayTranslation = cachedTranslation.TranslatedText;
            }
            else
            {
                // --- USE THE FALLBACK HERE ---
                // We send 'TextToTranslate' (which picks Description if Content is empty)
                string textToProcess = snapshot.TextToTranslate;

                // ... DeepL Call goes here ...
                // For now, simulate it:
                string simulated = $"[Translated from {textToProcess.Length} chars]: This is a simulated translation of '{textToProcess.Substring(0, Math.Min(20, textToProcess.Length))}...' into {TargetLanguage}.";

                var newTrans = new Translation
                {
                    LanguageCode = TargetLanguage,
                    TranslatedText = simulated,
                    ArticleSnapshotId = snapshot.Id
                };

                _db.Translations.Add(newTrans);
                await _db.SaveChangesAsync();

                Snapshot.DisplayTranslation = simulated;
            }

            Snapshot.TargetLanguage = TargetLanguage;
            return Page();
        }
    }
}