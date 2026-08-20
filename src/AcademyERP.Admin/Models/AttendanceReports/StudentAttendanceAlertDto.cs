namespace AcademyERP.Admin.Models.AttendanceReports;

public class StudentAttendanceAlertDto
{
    public string StudentName { get; set; } = string.Empty;

    public string CourseName { get; set; } = string.Empty;

    public decimal AttendancePercentage { get; set; }
}