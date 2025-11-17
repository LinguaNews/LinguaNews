using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class NewsController : ControllerBase
{
	private readonly INewsApiAiService _newsService;

	// Constructor injection
	public NewsController(INewsApiAiService newsService)
	{
		_newsService = newsService;
	}

	// GET api/news/search
	[HttpGet("search")]
	public async Task<ActionResult<IReadOnlyList<NewsAiArticle>>> Search(string? q, CancellationToken ct)
	{
		var articles = await _newsService.GetArticlesAsync(q, ct);
		return Ok(articles);
	}
}
