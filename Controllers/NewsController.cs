using LinguaNews;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;

[ApiController]
[Route("api/[controller]")]
public class NewsController : ControllerBase
/*{
	private readonly INewsApiAiService _newsService;

	// Constructor injection
	public NewsController(INewsApiAiService newsService)
	{
		_newsService = newsService;
	}

	// GET api/news/search
	[HttpGet("search")]
	public async Task<ActionResult<IReadOnlyList<ArticleData>>> Search(string? q, CancellationToken ct)
	{
		var articles = await _newsService.GetArticlesAsync(q, ct);
		return Ok(articles);
	}
}*/
// Factory method to fetch and deserialize directly from NewsData.io
{
	public static async Task<ArticleData[]> FromNewsDataAsync(string apiKey, string query)
	{
		using (var client = new HttpClient())
		{
			// Example endpoint: https://newsdata.io/api/1/news?apikey=<apiKey>&q=<QUERY>
			var url = $"https://newsdata.io/api/1/news?apikey={apiKey}&q={query}";
			var json = await client.GetStringAsync(url);

			// Use ArticleData.FromJson to get ArticleData[]
			var articles = ArticleData.FromJson(json);
			return articles ?? Array.Empty<ArticleData>();
		}
	}
}