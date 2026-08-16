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
                d => d.AttendanceDate,
                o => o.MapFrom(s => s.AttendanceDate)
            )
            .ForMember(
                d => d.Status,
                o => o.MapFrom(s => s.Status)
            )
            .ForMember(
                d => d.Remarks,
                o => o.MapFrom(s => s.Remarks)
            )
            .ForMember(
                d => d.StudentId,
                o => o.MapFrom(s => s.Enrollment.StudentId)
            )
            .ForMember(
                d => d.StudentName,
                o => o.MapFrom(s => s.Enrollment.Student.FullName)
            )
            .ForMember(
                d => d.TeacherId,
                o => o.MapFrom(s => s.Enrollment.TeacherId!.Value)
            )
            .ForMember(
                d => d.TeacherName,
                o => o.MapFrom(s => s.Enrollment.Teacher!.FullName)
            )
            .ForMember(
                d => d.ProgramId,
                o => o.MapFrom(s => s.Enrollment.Course.ProgramId)
            )
            .ForMember(
                d => d.ProgramName,
                o => o.MapFrom(s => s.Enrollment.Course.Program.ProgramName)
            )
            .ForMember(
                d => d.CourseName,
                o => o.MapFrom(s => s.Enrollment.Course.CourseName)
            );
    }
}