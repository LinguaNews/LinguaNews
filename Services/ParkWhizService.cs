using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using LinguaNews.Config;
using LinguaNews.Models;
using Microsoft.Extensions.Options;

namespace LinguaNews.Services
{
    public class ParkWhizService : IParkWhizService
    {
        private readonly HttpClient _http;
        private readonly ParkWhizOptions _opts;

        public ParkWhizService(HttpClient http, IOptions<ParkWhizOptions> opts)
        {
            _http = http ?? throw new ArgumentNullException(nameof(http));
            _opts = opts?.Value ?? throw new ArgumentNullException(nameof(opts));
        }

        public async Task<IReadOnlyList<ParkLocation>> SearchAsync(double latitude, double longitude, int? radiusMeters, string? query, CancellationToken ct)
        {
            radiusMeters ??= _opts.RadiusMeters;

            var uri = $"parking-lots?lat={latitude.ToString(CultureInfo.InvariantCulture)}&lng={longitude.ToString(CultureInfo.InvariantCulture)}&radius={radiusMeters}";
            if (!string.IsNullOrWhiteSpace(query))
            {
                uri += $"&q={Uri.EscapeDataString(query)}";
            }

            using var req = new HttpRequestMessage(HttpMethod.Get, uri);
            using var res = await _http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);
            res.EnsureSuccessStatusCode();

            await using var stream = await res.Content.ReadAsStreamAsync(ct).ConfigureAwait(false);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct).ConfigureAwait(false);

            var root = doc.RootElement;
            JsonElement itemsElement = root;

            if (root.ValueKind == JsonValueKind.Object)
            {
                if (root.TryGetProperty("results", out var r)) itemsElement = r;
                else if (root.TryGetProperty("data", out var d)) itemsElement = d;
                else
                {
                    foreach (var prop in root.EnumerateObject())
                    {
                        if (prop.Value.ValueKind == JsonValueKind.Array)
                        {
                            itemsElement = prop.Value;
                            break;
                        }
                    }
                }
            }

            var list = new List<ParkLocation>();
            if (itemsElement.ValueKind != JsonValueKind.Array) return list;

            foreach (var item in itemsElement.EnumerateArray())
            {
                try
                {
                    var pl = new ParkLocation { RawJson = item.GetRawText() };

                    if (item.TryGetProperty("id", out var idP) && idP.ValueKind == JsonValueKind.String) pl.Id = idP.GetString();
                    if (item.TryGetProperty("name", out var nameP) && nameP.ValueKind == JsonValueKind.String) pl.Name = nameP.GetString();
                    if (item.TryGetProperty("address", out var addrP) && addrP.ValueKind == JsonValueKind.String) pl.Address = addrP.GetString();

                    if (item.TryGetProperty("location", out var loc) && loc.ValueKind == JsonValueKind.Object)
                    {
                        if (loc.TryGetProperty("lat", out var latP) && latP.TryGetDouble(out var lat)) pl.Latitude = lat;
                        if (loc.TryGetProperty("lng", out var lngP) && lngP.TryGetDouble(out var lng)) pl.Longitude = lng;
                    }
                    else
                    {
                        if (item.TryGetProperty("lat", out var latP) && latP.TryGetDouble(out var lat)) pl.Latitude = lat;
                        if (item.TryGetProperty("lng", out var lngP) && lngP.TryGetDouble(out var lng)) pl.Longitude = lng;
                    }

                    if (item.TryGetProperty("price", out var priceP))
                    {
                        if (priceP.ValueKind == JsonValueKind.Number && priceP.TryGetDecimal(out var dec)) pl.Price = dec;
                        else if (priceP.ValueKind == JsonValueKind.Object && priceP.TryGetProperty("amount", out var amt) && amt.TryGetDecimal(out var amtDec)) pl.Price = amtDec;
                    }

                    if (item.TryGetProperty("distance", out var distP) && distP.TryGetDouble(out var dist)) pl.DistanceMeters = dist;

                    list.Add(pl);
                }
                catch
                {
                    // skip malformed item
                }
            }

            return list;
        }
    }
}