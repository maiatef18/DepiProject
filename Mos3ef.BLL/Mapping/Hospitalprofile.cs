using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Mos3ef.BLL.Dtos.Hospital;
using Mos3ef.BLL.Dtos.Review;
using Mos3ef.BLL.Dtos.Services;
using Mos3ef.DAL.Extensions;
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

            
            CreateMap<Service, ServicesAddDto>().ReverseMap();
            CreateMap<Service, ServiceShowDto>().ReverseMap();
            CreateMap<Service, ServiceHospitalDto>().ReverseMap();
            CreateMap<Service, ServicesUpdateDto>().ReverseMap();
            CreateMap<Service, ServiceReadDto>()
             .ForMember(dest => dest.HospitalName,
                 opt => opt.MapFrom(src => src.Hospital.Name))
             .ForMember(dest => dest.HospitalLatitude,
                 opt => opt.MapFrom(src => src.Hospital.Latitude))
             .ForMember(dest => dest.HospitalLongitude,
                 opt => opt.MapFrom(src => src.Hospital.Longitude))

             .ForMember(dest => dest.HospitalImage,
                 opt => opt.MapFrom(src => src.Hospital.ImageUrl))  

             .ForMember(dest => dest.CategoryName,
                 opt => opt.MapFrom(src => src.Category.GetDisplayName()));



            CreateMap<Review, ReviewReadDto>().ReverseMap();

        }
    }
}
