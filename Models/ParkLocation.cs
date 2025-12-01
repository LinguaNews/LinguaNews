using System;

namespace LinguaNews.Models
{
    public sealed class ParkLocation
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public decimal? Price { get; set; }
        public double? DistanceMeters { get; set; }
        public string? RawJson { get; set; }
    }
}