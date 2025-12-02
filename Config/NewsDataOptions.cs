namespace LinguaNews.Options
{
    public class NewsDataOptions
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public int PageSize { get; set; } = 9;
        public string Language { get; set; } = "en";
        //public int LastHowManyDays { get; set; } = 7;
    }
}
