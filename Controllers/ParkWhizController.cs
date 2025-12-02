using System.Threading;
using System.Threading.Tasks;
using LinguaNews.Services;
using Microsoft.AspNetCore.Mvc;

namespace LinguaNews.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParkWhizController : ControllerBase
    {
        private readonly IParkWhizService _park;

        public ParkWhizController(IParkWhizService park)
        {
            _park = park;
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(double lat, double lng, int? radius, string? q, CancellationToken ct)
        {
            var results = await _park.SearchAsync(lat, lng, radius, q, ct);
            return Ok(results);
        }
    }
}