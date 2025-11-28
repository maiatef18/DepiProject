using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Mos3ef.BLL.Dtos.Hospital;
using Mos3ef.BLL.Dtos.Review;
using Mos3ef.BLL.Dtos.Services;
using Mos3ef.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mos3ef.BLL.Mapping
{
    public class Hospitalprofile : Profile
    {
        public Hospitalprofile()
        {
            CreateMap<Hospital, HospitalReadDto>().ReverseMap();
            CreateMap<HospitalAddDto, Hospital>().ReverseMap();
            CreateMap<HospitalUpdateDto, Hospital>().ReverseMap();

            CreateMap<Service, ServiceReadDto>().ReverseMap();
            CreateMap<Service, ServicesAddDto>().ReverseMap();
            CreateMap<Service, ServiceShowDto>().ReverseMap();
            CreateMap<Service, ServicesUpdateDto>().ReverseMap();

            CreateMap<Review, ReviewReadDto>().ReverseMap();

        }
    }
}
