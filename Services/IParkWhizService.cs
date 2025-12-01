using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LinguaNews.Models;

namespace LinguaNews.Services
{
    public interface IParkWhizService
    {
        Task<IReadOnlyList<ParkLocation>> SearchAsync(double latitude, double longitude, int? radiusMeters, string? query, CancellationToken ct);
    }
}