using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mos3ef.BLL.Dtos.ReviewDto
{
    public class ReviewUpdateDto
    {
        public int ReviewId { get; set; }

        public int Rating { get; set; }

        public DateTime Review_Date { get; set; }

        public string? Comment { get; set; }

        public int ServiceId { get; set; }

        public int PatientId { get; set; }
    }
}
