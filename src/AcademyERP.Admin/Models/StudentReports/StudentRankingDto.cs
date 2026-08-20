namespace AcademyERP.Admin.Models.StudentReports;

public class StudentRankingDto
{
    public int Rank { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public string CourseName { get; set; } = string.Empty;

    public decimal PerformanceScore { get; set; }

    public decimal Attendance{ get; set; }
}