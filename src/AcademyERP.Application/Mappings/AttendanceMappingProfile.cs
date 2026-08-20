using AcademyERP.Application.DTOs.Attendance;
using AcademyERP.Domain.Entities.Attendance;
using AutoMapper;

namespace AcademyERP.Application.Mappings;

public class AttendanceMappingProfile : Profile
{
    public AttendanceMappingProfile()
    {
        CreateMap<Attendance, AttendanceResponse>()

            .ForMember(
                dest => dest.ScheduledClassId,
                opt => opt.MapFrom(src => src.ScheduledClassId)
            )

            .ForMember(
                dest => dest.ClassDate,
                opt => opt.MapFrom(src => src.ScheduledClass.ClassDate)
            )

            .ForMember(
                dest => dest.StudentId,
                opt => opt.MapFrom(
                    src => src.ScheduledClass
                        .TeachingSchedule
                        .Enrollment
                        .StudentId)
            )

            .ForMember(
                dest => dest.StudentName,
                opt => opt.MapFrom(
                    src => src.ScheduledClass
                        .TeachingSchedule
                        .Enrollment
                        .Student
                        .FullName)
            )

            .ForMember(
                dest => dest.TeacherId,
                opt => opt.MapFrom(
                    src => src.ScheduledClass
                        .TeachingSchedule
                        .TeacherId)
            )

            .ForMember(
                dest => dest.TeacherName,
                opt => opt.MapFrom(
                    src => src.ScheduledClass
                        .TeachingSchedule
                        .Teacher
                        .FullName)
            )

            .ForMember(
                dest => dest.ProgramId,
                opt => opt.MapFrom(
                    src => src.ScheduledClass
                        .TeachingSchedule
                        .Enrollment
                        .Course
                        .ProgramId)
            )

            .ForMember(
                dest => dest.ProgramName,
                opt => opt.MapFrom(
                    src => src.ScheduledClass
                        .TeachingSchedule
                        .Enrollment
                        .Course
                        .Program
                        .ProgramName)
            )

                       .ForMember(
                dest => dest.CourseName,
                opt => opt.MapFrom(
                    src => src.ScheduledClass
                        .TeachingSchedule
                        .Enrollment
                        .Course
                        .CourseName)
            )

            .ForMember(
                dest => dest.StartTime,
                opt => opt.MapFrom(
                    src => src.ScheduledClass
                        .TeachingSchedule
                        .StartTime)
                        
            ) 
            .ForMember(
    dest => dest.EndTime,
    opt => opt.MapFrom(
        src =>
            src.ScheduledClass
              .TeachingSchedule
              .StartTime
              .AddMinutes(
                  src.ScheduledClass
                    .TeachingSchedule
                    .ClassDuration
                    .Minutes
              )
    )
);

            
    }
}