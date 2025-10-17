using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Mos3ef.DAL.Models
{
    public class Patient
    {
        [Key]
        public int PatientId { get; set; }

        [StringLength(100)]
        public string? Location { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        [Required]
        public string? UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;

        public ICollection<Review>? Reviews { get; set; }
        public ICollection<SavedService>? SavedServices { get; set; }
    }
}
