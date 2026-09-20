namespace AcademyERP.Admin.Models.Reports;

public class BestPerformingStudentResponse
{
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string TeacherName { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public double PerformanceScore { get; set; }
    public double AttendancePercentage { get; set; }
    public int ClassesReported { get; set; }
    public int AttendanceRecords { get; set; }
    public int Rank { get; set; }
}

public class BestPerformingStudentsReportResponse
{
    public int TotalStudents { get; set; }
    public int StudentsWithReports { get; set; }
    public double AveragePerformance { get; set; }
    public double AverageAttendance { get; set; }
    public BestPerformingStudentResponse? BestStudent { get; set; }
    public List<BestPerformingStudentResponse> Students { get; set; } = new();
}
