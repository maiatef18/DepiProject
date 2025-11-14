using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mos3ef.DAL.Enum;
using Mos3ef.DAL.Enums;

namespace Mos3ef.DAL.Models
{
    public class Service
    {
        [Key]
        public int ServiceId { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = null!;

        [StringLength(500)]
        public string? Description { get; set; }

        [Range(0, 100000)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [StringLength(50)]
        public string? Availability { get; set; }

        [StringLength(100)]
        public string? Working_Hours { get; set; }

        [Required]
        public CategoryType Category { get; set; }

        [Required]
        public int HospitalId { get; set; }

        public Hospital Hospital { get; set; } = null!;

        public ICollection<Review>? Reviews { get; set; }
        public ICollection<SavedService>? SavedServices { get; set; }
    }
}
