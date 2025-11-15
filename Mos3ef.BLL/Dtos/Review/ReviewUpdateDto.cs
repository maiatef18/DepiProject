using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mos3ef.BLL.Dtos.Review
{
    public class ReviewUpdateDto
    {

        public int ReviewId { get; set; }
        public int Rating { get; set; }

        public DateTime Review_Date { get; set; }

        public string? Comment { get; set; }

    }
}
