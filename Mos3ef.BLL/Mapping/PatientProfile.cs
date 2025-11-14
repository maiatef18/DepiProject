using AutoMapper;
using Mos3ef.BLL.Dtos.Patient;
using Mos3ef.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mos3ef.BLL.Mapping
{
    public class PatientProfile : Profile
    {
        public PatientProfile()
        {
            // Mapping for updating patient - ignores fields that shouldn't be updated
            CreateMap<PatientUpdateDto, Patient>()
                .ForMember(dest => dest.PatientId, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Reviews, opt => opt.Ignore())
                .ForMember(dest => dest.SavedServices, opt => opt.Ignore());
            
            // Mapping for reading patient data
            CreateMap<Patient, PatientReadDto>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User != null ? src.User.Email : string.Empty))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.User != null ? src.User.PhoneNumber : string.Empty));
        }
    }
}
