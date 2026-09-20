namespace AcademyERP.Admin.Models.ClassReports;

public class TeacherReportMonitoringResponse
{
    public Guid ScheduledClassId { get; set; }

    public DateOnly ClassDate { get; set; }

    public Guid TeacherId { get; set; }
public string TeacherName { get; set; } = string.Empty;
public string EmployeeCode { get; set; } = string.Empty;

    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;

    public string CourseName { get; set; } = string.Empty;

    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }

    public string ClassStatus { get; set; } = string.Empty;

public string AttendanceStatus { get; set; } = "Not Marked";

    public bool ReportSubmitted { get; set; }

    public DateTime? ReportSubmittedAt { get; set; }

    public Guid? ClassReportId { get; set; }
}