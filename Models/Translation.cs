namespace LinguaNews.Models
{
    public class Translation
    {
        public int Id { get; set; }

        // Attribute for which language this is
        public string LanguageCode { get; set; }

        // ATTRIBUTE MOVED HERE: This is the actual translated text
        public string TranslatedText { get; set; }

        // This links it back to the one article it belongs to
        public int ArticleSnapshotId { get; set; }
        public ArticleSnapshot ArticleSnapshot { get; set; }
    }
}