using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Mos3ef.DAL.Models
{
    public class Hospital
    {
        public int HospitalId { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; } = null!; 

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(100)]
        public string? Location { get; set; } = null!; 

        [Required]
        [StringLength(200)]
        public string Address { get; set; } = null!;

        [Phone]
        [StringLength(15)]
        public string? Phone_Number { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime Opening_Hours { get; set; } = DateTime.Now;

        [Url]
        public string? Website { get; set; } 
        public string? Region { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public string? UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;

        public ICollection<Service>? Services { get; set; }
    }
}
