using Microsoft.AspNetCore.Mvc;
using LinguaNews;
using LinguaNews.Services;
using LinguaNews.Models;


[Route("api/[controller]")]
[ApiController]
public class NewsController : ControllerBase
{
	private readonly INewsDataIngestService _newsService;

	// Constructor injection
	public NewsController(INewsDataIngestService newsService)
	{
		_newsService = newsService;
	}

	// GET api/news/search
	[HttpGet("search")]
	public async Task<ActionResult<IReadOnlyList<NewsDataArticle>>> Search([FromQuery]string? q,[FromQuery] string? language, CancellationToken ct)
	{
		var articles = await _newsService.GetArticlesAsync(q, language ?? "en", ct);
		return Ok(articles);
	}
}
