namespace AcademyERP.Application.DTOs.StudentReports;

public class BestStudentDto
{
    public string StudentName { get; set; } = string.Empty;

    public string CourseName { get; set; } = string.Empty;

    public decimal PerformanceScore { get; set; }

    public decimal AttendancePercentage { get; set; }
}