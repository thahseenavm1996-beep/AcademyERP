namespace AcademyERP.Application.DTOs.AttendanceReports;

public class TeacherAttendanceSummaryDto
{
    public string TeacherName { get; set; } = string.Empty;

    public int TotalClasses { get; set; }

    public decimal AttendancePercentage { get; set; }
}