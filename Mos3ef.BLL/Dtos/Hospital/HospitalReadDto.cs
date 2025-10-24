﻿using Mos3ef.BLL.Dtos.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mos3ef.BLL.Dtos.Hospital
{
    public class HospitalReadDto
    {
            public int Id { get; set; }
            public string Name { get; set; }
            public string? Description { get; set; }
            public string Location { get; set; }
            public string Address { get; set; }
            public string? Phone_Number { get; set; }
            public DateTime Opening_Hours { get; set; }
            public string? Website { get; set; }
            public List<ServiceReadDto>? Services { get; set; }

    }
}
