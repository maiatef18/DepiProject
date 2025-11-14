using AutoMapper;
using Mos3ef.BLL.Dtos.Review;
using Mos3ef.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mos3ef.BLL.Mapping
{
    public class ReviewProfile : Profile
    {
        public ReviewProfile()
        {
            // Create your mappings here
            // Example:
            // CreateMap<Source, Destination>();
            CreateMap<Review, ReviewReadDto>().ReverseMap();
            CreateMap<ReviewAddDto, Review>();
            CreateMap<ReviewUpdateDto, Review>().ReverseMap();

        }
    }
}
