using System;
using System.ComponentModel.DataAnnotations;

namespace Mos3ef.DAL.Models
{
    public class SavedService
    {
        [Key]
        public int SavedId { get; set; }

        [Required]
        public DateTime Saved_Date { get; set; } = DateTime.Now;

        [Required]
        public int ServiceId { get; set; }
        public Service Service { get; set; } = null!;

        [Required]
        public int PatientId { get; set; }
        public Patient Patient { get; set; } = null!;
    }
}
