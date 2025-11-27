using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LinguaNews.Models
{
    // This class maps directly to a Database Table for caching
    public class ArticleSnapshot
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string OriginalUrl { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Author { get; set; } = string.Empty;

        public string SourceName { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime FetchedAt { get; set; } = DateTime.UtcNow;

        // --- View-Only Properties (Not stored in Database) ---
        // We use [NotMapped] so Entity Framework ignores them. 
        // These are useful for passing data to the View without creating a new table column.

        [NotMapped]
        public string DisplayTranslation { get; set; } = string.Empty;

        [NotMapped]
        public string TargetLanguage { get; set; } = "ES";

        // If you decide to implement the Translations table later, uncomment this:
        // public List<Translation> Translations { get; set; } = new List<Translation>();
    }
}


