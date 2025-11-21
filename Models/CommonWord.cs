using System.ComponentModel.DataAnnotations;

namespace LinguaNews.Models
{
	public class CommonWord
	{
		public int Id { get; set; }

		[Required]
		public string OriginalWord { get; set; } = string.Empty;

		[Required]
		public string LanguageCode { get; set; } = string.Empty; // e.g., "ES", "FR"

		[Required]
		public string Translation { get; set; } = string.Empty;
	}
}