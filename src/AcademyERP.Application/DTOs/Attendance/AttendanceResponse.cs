using AcademyERP.Domain.Enums;

namespace AcademyERP.Application.DTOs.Attendance;

public class AttendanceResponse
{
    public Guid Id { get; set; }

    public Guid ScheduledClassId { get; set; }

    public Guid StudentId { get; set; }

    public string StudentName { get; set; } = string.Empty;


    public Guid TeacherId { get; set; }

    public string TeacherName { get; set; } = string.Empty;


    public Guid ProgramId { get; set; }

    public string ProgramName { get; set; } = string.Empty;


    public string CourseName { get; set; } = string.Empty;


    public DateOnly ClassDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }


    public AttendanceStatus Status { get; set; }

    public string? Remarks { get; set; }
}