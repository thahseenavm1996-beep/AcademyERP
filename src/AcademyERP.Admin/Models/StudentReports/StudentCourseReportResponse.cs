namespace AcademyERP.Admin.Models.StudentReports;

public class StudentCourseReportResponse
{
    public string CourseName { get; set; } = string.Empty;

    public string TeacherName { get; set; } = string.Empty;

    public DateOnly StartDate { get; set; }

    public string Status { get; set; } = string.Empty;
}