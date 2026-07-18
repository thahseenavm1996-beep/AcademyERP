namespace AcademyERP.Application.DTOs.TeachingSchedules;

public class TeachingScheduleResponse
{
    public Guid Id { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public string TeacherName { get; set; } = string.Empty;

    public string CourseName { get; set; } = string.Empty;

    public DayOfWeek DayOfWeek { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }
}