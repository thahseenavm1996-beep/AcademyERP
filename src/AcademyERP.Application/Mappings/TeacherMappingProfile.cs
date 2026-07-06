using AutoMapper;
using AcademyERP.Application.DTOs.Teachers;
using AcademyERP.Domain.Entities.Teachers;

namespace AcademyERP.Application.Mappings;

public class TeacherMappingProfile : Profile
{
    public TeacherMappingProfile()
    {
        // DTO -> Entity
        CreateMap<CreateTeacherRequest, Teacher>();

        CreateMap<UpdateTeacherRequest, Teacher>();

        // Entity -> DTO
        CreateMap<Teacher, TeacherResponse>();
    }
}