using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LinguaNews.Models
{
    [Table("Translations")]
    public class Translation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(5)]
        public string LanguageCode { get; set; } = string.Empty;
        [JsonPropertyName("text")]
        public string TranslatedText { get; set; } = string.Empty;

        // Foreign Key
        public int ArticleSnapshotId { get; set; }

        [ForeignKey("ArticleSnapshotId")]
        public ArticleSnapshot? ArticleSnapshot { get; set; }
    }
}
