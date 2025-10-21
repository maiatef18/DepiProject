using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Mos3ef.BLL.Dtos.Auth;
using Mos3ef.DAL.Enums;
using Mos3ef.DAL.Models;

namespace Mos3ef.BLL.Mapping
{
    public class AuthProfile : Profile
    {
        public AuthProfile()
        {

            CreateMap<PatientRegisterDto, ApplicationUser>()
     .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email)) 
     .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
     
     .ForMember(dest => dest.UserType, opt => opt.MapFrom(src => UserType.Patient));

           
            CreateMap<PatientRegisterDto, Patient>()
    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
            ;


            CreateMap<HospitalRegisterDto, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email)) 
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.UserType, opt => opt.MapFrom(src => UserType.Hospital));

            CreateMap<HospitalRegisterDto, Hospital>();


            CreateMap<ApplicationUser, AuthResponseDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.UserType, opt => opt.MapFrom(src => src.UserType.ToString()))
                .ForMember(dest => dest.PatientProfileId, opt => opt.MapFrom(src => src.PatientProfile != null ? src.PatientProfile.PatientId : (int?)null))
                .ForMember(dest => dest.HospitalProfileId, opt => opt.MapFrom(src => src.HospitalProfile != null ? src.HospitalProfile.HospitalId : (int?)null));

        }
    }
    
}
