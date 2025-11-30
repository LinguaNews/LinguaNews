using LinguaNews.Data;
using LinguaNews.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LinguaNews.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoryController : ControllerBase
    {
        private readonly LinguaNewsDbContext _db;

        public HistoryController(LinguaNewsDbContext db)
        {
            _db = db;
        }

        // GET: api/History
        // Returns the last 10 articles viewed by users as JSON
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArticleSnapshot>>> GetHistory()
        {
            return await _db.ArticleSnapshots
                //.Include(a => a.Translations) // <--- LOAD THE TRANSLATIONS
                .OrderByDescending(a => a.FetchedAt)
                .Take(10)
                .ToListAsync();
        }
    }
}
