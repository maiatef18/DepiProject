﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mos3ef.BLL.Dtos.Review
{
    public class ReviewReadDto
    {
        public int Rating { get; set; }

        public DateTime Review_Date { get; set; } = DateTime.Now;

        public string? Comment { get; set; }
    }
}
