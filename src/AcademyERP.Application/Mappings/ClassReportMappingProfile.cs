using AcademyERP.Application.DTOs.ClassReports;
using AcademyERP.Domain.Entities.ClassReports;
using AutoMapper;

namespace AcademyERP.Application.Mappings;

public class ClassReportMappingProfile : Profile
{
    public ClassReportMappingProfile()
    {
        CreateMap<CreateClassReportRequest, ClassReport>();

        CreateMap<UpdateClassReportRequest, ClassReport>();

        CreateMap<ClassReport, ClassReportResponse>()
            .ForMember(dest => dest.TeacherName,
                opt => opt.MapFrom(src => src.Teacher.FullName))

            .ForMember(dest => dest.StudentName,
                opt => opt.MapFrom(src => src.Student.FullName))

            .ForMember(dest => dest.CourseName,
                opt => opt.MapFrom(src => src.Enrollment.Course.CourseName));
    }
}