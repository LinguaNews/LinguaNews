using System;

namespace LinguaNews.Config
{
    public class ParkWhizOptions
    {
        public string? BaseUrl { get; set; }
        public string? ApiKey { get; set; }
        public int RadiusMeters { get; set; } = 1000;
    }
}