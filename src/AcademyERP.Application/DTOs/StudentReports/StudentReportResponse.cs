namespace AcademyERP.Application.DTOs.StudentReports;

public class StudentReportResponse
{
    public Guid StudentId { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public string? ParentName { get; set; }

    public List<StudentCourseReportResponse> Courses { get; set; }
        = new();

    public AttendanceSummaryResponse Attendance { get; set; }
        = new();

    public LearningProgressSummaryResponse Progress { get; set; }
        = new();

    public HomeworkSummaryResponse Homework { get; set; }
        = new();

    public List<string> TeacherFeedback { get; set; }
        = new();
}