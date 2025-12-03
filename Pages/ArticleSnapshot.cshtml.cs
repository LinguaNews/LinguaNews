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
        //private readonly INewsDataIngestService _ingestService;
        private readonly ITranslationService _translationService;

        public ArticleSnapshotModel(
            LinguaNewsDbContext db,
            //INewsDataIngestService ingestService,
            ITranslationService translationService)
        {
            _db = db;
            //_ingestService = ingestService;
            _translationService = translationService;
        }

        // --- INPUTS FROM URL ---
        [BindProperty(SupportsGet = true)]
        public string ArticleUrl { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public string? IncomingTitle { get; set; }              // IncomingX are from Index page

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

        public Dictionary<string, string> SentenceMap { get; set; } = []; // Maps original sentences to translated sentences

        public async Task<IActionResult> OnGetAsync() //GET handler, Load/Save article snapshot for caching
        {
            if (string.IsNullOrWhiteSpace(ArticleUrl)) return RedirectToPage("/Index");

            // Check if we already have this article saved
            var existingSnapshot = await _db.ArticleSnapshots
                .Include(a => a.Translations)
                .FirstOrDefaultAsync(a => a.ArticleUrl == ArticleUrl);
            // If new, Create it from URL Data
            if (existingSnapshot == null)
            {
                existingSnapshot = new ArticleSnapshot
                {
                    ArticleUrl = ArticleUrl,
                    Title = IncomingTitle ?? "Unknown Title",
                    Description = IncomingDescription ?? string.Empty,
                    Content = string.Empty, // Content is empty (Free Tier Strategy, add to ViewModel as hidden when upgraded)
                    SourceName = IncomingSource ?? "Unknown",
                    ImageUrl = IncomingImage ?? string.Empty,
                    FetchedAt = DateTime.UtcNow
                };

                _db.ArticleSnapshots.Add(existingSnapshot);
                await _db.SaveChangesAsync();
            }

            Snapshot = existingSnapshot;

            var cachedTrans = Snapshot.Translations.FirstOrDefault(t => t.LanguageCode == TargetLanguage);
            if (cachedTrans != null)
            {
                Snapshot.DisplayTranslation = cachedTrans.TranslatedText;
                BuildSentenceMap(
                    Snapshot.TextToTranslate,
                    cachedTrans.TranslatedText);
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync() //POST handler, Translate article snapshot
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
                // Call DeepL
                // TextToTranslate automatically picks Description (since Content is empty)
                string textToProcess = snapshot.TextToTranslate;

                var translatedText = await _translationService.TranslateAsync(textToProcess, TargetLanguage);

                // Save
                var newTrans = new Translation
                {
                    LanguageCode = TargetLanguage,
                    TranslatedText = translatedText,
                    ArticleSnapshotId = snapshot.Id
                };

                _db.Translations.Add(newTrans);
                await _db.SaveChangesAsync();

                Snapshot.DisplayTranslation = translatedText;
            }
            BuildSentenceMap(snapshot.TextToTranslate, Snapshot.DisplayTranslation);
            return Page();
        }
        // --- HELPER: Sentence Splitter --- [AI GENERATED]
        private void BuildSentenceMap(string original, string translated)
        {
            char[] splitters = { '.', '!', '?' };

            var originalSentences = original.Split(splitters, StringSplitOptions.RemoveEmptyEntries)
                                            .Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToArray();

            var translatedSentences = translated.Split(splitters, StringSplitOptions.RemoveEmptyEntries)
                                                .Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToArray();

            int max = Math.Min(originalSentences.Length, translatedSentences.Length);

            for (int i = 0; i < max; i++)
            {
                string key = $"{i}_{originalSentences[i]}";
                if (!SentenceMap.ContainsKey(key))
                {
                    SentenceMap[key] = translatedSentences[i];
                }
            }
        }
    }
}