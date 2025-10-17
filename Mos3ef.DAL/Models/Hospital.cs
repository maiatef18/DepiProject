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

        [Required]
        [StringLength(100)]
        public string Location { get; set; } = null!; // e.g., city or area

        [Required]
        [StringLength(200)]
        public string Address { get; set; } = null!;

        [Phone]
        [StringLength(15)]
        public string? Phone_Number { get; set; }

        [DataType(DataType.Date)]
        public DateTime Opening_Date { get; set; } = DateTime.Now;

        [Url]
        public string? Website { get; set; }

        public int UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;

        public ICollection<Service>? Services { get; set; }
    }
}
