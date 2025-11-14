using System.ComponentModel.DataAnnotations;

namespace Mos3ef.BLL.Dtos.Services
{
    public class ServiceFilterDto
    {
        // --- New, Specific Filters ---
        public bool? HasEmergency { get; set; }
        public bool? HasIcu { get; set; }
        public bool? HasNicu { get; set; }

        // --- Other Filters ---
        [StringLength(100, ErrorMessage = "Hospital name cannot exceed 100 characters.")]
        public string? HospitalName { get; set; }

        [Range(0, 1000000, ErrorMessage = "Max price must be between 0 and 1,000,000.")]
        public decimal? MaxPrice { get; set; }

        [Range(1, 5, ErrorMessage = "Minimum rating must be between 1 and 5.")]
        public int? MinRating { get; set; }

        [StringLength(100, ErrorMessage = "Region cannot exceed 100 characters.")]
        public string? Region { get; set; }

        public bool OnlyAvailable { get; set; } = true;

        [StringLength(200, ErrorMessage = "Keyword cannot exceed 200 characters.")]
        public string? Keyword { get; set; }

        // --- Sorting ---
        [RegularExpression("^(price|name|rating|distance)$", ErrorMessage = "SortBy must be one of: price, name, rating, distance")]
        public string? SortBy { get; set; }

        public bool IsAscending { get; set; } = true;

        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90.")]
        public double? UserLatitude { get; set; }

        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180.")]
        public double? UserLongitude { get; set; }

        [Range(0, 10000, ErrorMessage = "Radius must be between 0 and 10,000 km.")]
        public double? RadiusKm { get; set; }

        // --- Pagination ---
        [Range(1, int.MaxValue, ErrorMessage = "Page number must be at least 1.")]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100.")]
        public int PageSize { get; set; } = 10;
    }
}
