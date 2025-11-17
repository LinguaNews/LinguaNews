using LinguaNews;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;

[ApiController]
[Route("api/[controller]")]
public class NewsController : ControllerBase
{
	private readonly INewsApiService _newsService;

	// Constructor injection
	public NewsController(INewsApiService newsService)
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