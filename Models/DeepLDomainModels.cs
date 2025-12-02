using System.Text.Json.Serialization;

namespace LinguaNews.Models
{
    public class DeepLResponse
    {
        [JsonPropertyName("translations")]
        public List<Translation>? Translations { get; set; }
    }
}
