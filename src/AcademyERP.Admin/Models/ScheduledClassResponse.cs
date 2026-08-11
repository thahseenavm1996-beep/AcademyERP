namespace AcademyERP.Admin.Models;

public class ScheduledClassResponse
{
    public Guid Id { get; set; }

    public DateOnly ClassDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public string TeacherName { get; set; } = string.Empty;

    public string CourseName { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;
}