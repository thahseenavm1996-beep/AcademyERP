namespace AcademyERP.Application.DTOs.TeacherReports;

public class TeacherWorkloadDto
{
    public string TeacherName { get; set; } = string.Empty;

    public int StudentsCount { get; set; }

    public int ActiveClasses { get; set; }

    public decimal WeeklyHours { get; set; }
}