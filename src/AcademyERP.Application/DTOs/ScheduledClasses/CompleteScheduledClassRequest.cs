using AcademyERP.Domain.Enums;

namespace AcademyERP.Application.DTOs.ScheduledClasses;

public class CompleteScheduledClassRequest
{
    public AttendanceStatus Attendance { get; set; }

    public string? LessonCovered { get; set; }

    public string? HomeworkStatus { get; set; }

    public string? TeacherRemarks { get; set; }
}