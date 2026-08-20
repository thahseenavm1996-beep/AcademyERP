namespace AcademyERP.Application.DTOs.AttendanceReports;

public class StudentAttendanceAlertDto
{
    public string StudentName { get; set; } = string.Empty;

    public string CourseName { get; set; } = string.Empty;

    public decimal AttendancePercentage { get; set; }
}