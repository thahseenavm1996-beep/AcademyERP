namespace AcademyERP.Admin.Models.AttendanceReports;

public class CourseAttendanceDto
{
    public string CourseName { get; set; } = string.Empty;

    public decimal AttendancePercentage { get; set; }
}