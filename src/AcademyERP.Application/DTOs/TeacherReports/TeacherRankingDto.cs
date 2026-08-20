namespace AcademyERP.Application.DTOs.TeacherReports;

public class TeacherRankingDto
{
    public int Rank { get; set; }

    public string TeacherName { get; set; } = string.Empty;

    public decimal PerformanceScore { get; set; }

    public int CompletedClasses { get; set; }

    public decimal AttendancePercentage { get; set; }

    public int StudentsCount { get; set; }
}