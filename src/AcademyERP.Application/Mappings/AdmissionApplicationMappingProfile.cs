using AcademyERP.Application.DTOs.Admissions;
using AcademyERP.Domain.Entities.Admissions;
using AutoMapper;

namespace AcademyERP.Application.Mappings;

public class AdmissionApplicationMappingProfile : Profile
{
    public AdmissionApplicationMappingProfile()
    {
        CreateMap<CreateAdmissionApplicationRequest, AdmissionApplication>();

        CreateMap<AdmissionApplication, AdmissionApplicationResponse>()
    .ForMember(
        dest => dest.ProgramName,
        opt => opt.MapFrom(src => src.Program.ProgramName))
    .ForMember(
        dest => dest.CourseName,
        opt => opt.MapFrom(src =>
            src.Course != null
                ? src.Course.CourseName
                : null));
    }
}