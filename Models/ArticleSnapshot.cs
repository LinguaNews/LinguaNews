using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LinguaNews.Models
{
    [Table("ArticleSnapshots")]
    public class ArticleSnapshot
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(2048)]
        public string ArticleUrl { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(255)]
        public string SourceName { get; set; } = string.Empty;

        [MaxLength(2048)]
        public string ImageUrl { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        // The full body (not used in testing / free tier)
        public string Content { get; set; } = string.Empty;

        public DateTime FetchedAt { get; set; } = DateTime.UtcNow;

        public List<Translation> Translations { get; set; } = new();

        // --- SMART HELPER ---
        // This is what we will send to DeepL. 
        // If Content is empty, it automatically uses Description.
        [NotMapped]
        public string TextToTranslate => !string.IsNullOrWhiteSpace(Content) ? Content : Description;

        // View Helpers
        [NotMapped]
        public string DisplayTranslation { get; set; } = string.Empty;
    }
}

