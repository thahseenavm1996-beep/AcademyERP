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

            .ForMember(
                dest => dest.TeacherId,
                opt => opt.MapFrom(src =>
                    src.ScheduledClass
                        .TeachingSchedule
                        .TeacherId)
            )

            .ForMember(
                dest => dest.TeacherName,
                opt => opt.MapFrom(src =>
                    src.ScheduledClass
                        .TeachingSchedule
                        .Teacher
                        .FullName)
            )


            .ForMember(
                dest => dest.StudentId,
                opt => opt.MapFrom(src =>
                    src.ScheduledClass
                        .TeachingSchedule
                        .Enrollment
                        .StudentId)
            )


            .ForMember(
                dest => dest.StudentName,
                opt => opt.MapFrom(src =>
                    src.ScheduledClass
                        .TeachingSchedule
                        .Enrollment
                        .Student
                        .FullName)
            )


            .ForMember(
                dest => dest.CourseName,
                opt => opt.MapFrom(src =>
                    src.ScheduledClass
                        .TeachingSchedule
                        .Enrollment
                        .Course
                        .CourseName)
            )


            .ForMember(
                dest => dest.ClassDate,
                opt => opt.MapFrom(src =>
                    src.ScheduledClass.ClassDate)
            );
    }
}