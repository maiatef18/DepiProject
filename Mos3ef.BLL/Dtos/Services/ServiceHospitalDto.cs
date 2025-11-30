using Mos3ef.DAL.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mos3ef.BLL.Dtos.Services
{
    public class ServiceHospitalDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? Availability { get; set; }
        public string? Working_Hours { get; set; }
        public CategoryType Category { get; set; }
        public double? AverageRating { get; set; }
    }
}
