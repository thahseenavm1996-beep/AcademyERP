using AutoMapper;
using AcademyERP.Application.DTOs.Teachers;
using AcademyERP.Domain.Entities.Teachers;
using AcademyERP.Domain.Enums;

namespace AcademyERP.Application.Mappings;

public class TeacherMappingProfile : Profile
{
    public TeacherMappingProfile()
    {
        // DTO -> Entity
        CreateMap<CreateTeacherRequest, Teacher>();

        CreateMap<UpdateTeacherRequest, Teacher>();

        // Entity -> DTO
       CreateMap<Teacher, TeacherResponse>()

    .ForMember(
        dest => dest.IsActive,
        opt => opt.MapFrom(
            src => src.Status == UserStatus.Active
        )
    );
    }
}