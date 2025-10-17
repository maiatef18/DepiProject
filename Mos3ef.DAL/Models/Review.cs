using System;
using System.ComponentModel.DataAnnotations;

namespace Mos3ef.DAL.Models
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        public DateTime Review_Date { get; set; } = DateTime.Now;

        [StringLength(300)]
        public string? Comment { get; set; }

        [Required]
        public int ServiceId { get; set; }
        public Service Service { get; set; } = null!;

        [Required]
        public int PatientId { get; set; }
        public Patient Patient { get; set; } = null!;
    }
}
