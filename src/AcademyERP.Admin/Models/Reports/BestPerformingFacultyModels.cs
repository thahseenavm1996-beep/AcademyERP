namespace AcademyERP.Admin.Models.Reports;

public class BestPerformingFacultyResponse
{
    public Guid TeacherId { get; set; }

    public string TeacherName { get; set; } = string.Empty;

    public string Gender { get; set; } = string.Empty;

    public string CourseName { get; set; } = string.Empty;

    public double PerformanceScore { get; set; }

    public double AttendancePercentage { get; set; }

    public int StudentsTaught { get; set; }

    public int ClassesReported { get; set; }

    public int AttendanceRecords { get; set; }

    public int Rank { get; set; }
}

public class BestPerformingFacultyReportResponse
{
    public int TotalFaculty { get; set; }

    public int FacultyWithReports { get; set; }

    public double AveragePerformance { get; set; }

    public double AverageAttendance { get; set; }

    public BestPerformingFacultyResponse? BestFaculty { get; set; }

    public List<BestPerformingFacultyResponse> Faculty { get; set; } = new();
}
