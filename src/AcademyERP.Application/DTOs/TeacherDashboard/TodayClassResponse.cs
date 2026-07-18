using AcademyERP.Domain.Enums;

namespace AcademyERP.Application.DTOs.TeacherDashboard;

public class TodayClassResponse
{
    public Guid ScheduledClassId { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public string CourseName { get; set; } = string.Empty;

    public DateOnly ClassDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public ScheduledClassStatus Status { get; set; }
}