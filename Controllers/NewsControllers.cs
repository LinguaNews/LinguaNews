using Microsoft.AspNetCore.Mvc;
using LinguaNews;
using LinguaNews.Services;

[ApiController]
[Route("api/[controller]")]
public class NewsController : ControllerBase
{
	private readonly INewsAPIAIService _newsService;

	// Constructor injection
	public NewsController(INewsAPIAIService newsService)
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
}
