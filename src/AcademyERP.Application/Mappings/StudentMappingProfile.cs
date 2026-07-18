using AcademyERP.Application.DTOs.Students;
using AcademyERP.Domain.Entities.Students;
using AutoMapper;

namespace AcademyERP.Application.Mappings;

public class StudentMappingProfile : Profile
{
    public StudentMappingProfile()
    {
        CreateMap<Student, StudentResponse>()
    .ForMember(
        dest => dest.Gender,
        opt => opt.MapFrom(src => src.Gender.ToString()));

        CreateMap<CreateStudentRequest, Student>();

        CreateMap<UpdateStudentRequest, Student>();
    }
}