using Mos3ef.BLL.Dtos.Services;

namespace Mos3ef.BLL.Dtos.Compare
{
    public class CompareResponseDto
    {
        public ServiceReadDto? Service1 { get; set; }
        public ServiceReadDto? Service2 { get; set; }
        
        // Comparison Metrics
        public ComparisonMetrics? Metrics { get; set; }
    }
}

namespace Mos3ef.BLL.Dtos.Compare
{
    public class ComparisonMetrics
    {
        public decimal? PriceDifference { get; set; }
        public string? PriceComparison { get; set; } // "Service1 is cheaper", "Service2 is cheaper", "Same price"
        
        public double? RatingDifference { get; set; }
        public string? RatingComparison { get; set; } // "Service1 has better rating", etc.
        
        public double? DistanceDifference { get; set; }
        public string? DistanceComparison { get; set; } // "Service1 is closer", etc.
        
        public string? AvailabilityComparison { get; set; } // "Both available", "Service1 available", etc.
        
        public string? Recommendation { get; set; } // Overall recommendation based on all factors
    }
}
