﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mos3ef.BLL.Dtos.Auth
{
   
        public class BasicResponseDto
        {
            public bool IsSuccess { get; set; }
            public string Message { get; set; } = string.Empty;
        }
    
}
