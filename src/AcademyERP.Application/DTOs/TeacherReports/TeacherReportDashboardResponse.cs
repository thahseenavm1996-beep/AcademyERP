namespace AcademyERP.Application.DTOs.TeacherReports;

public class TeacherReportDashboardResponse
{
    public int TotalTeachers { get; set; }

    public int ActiveTeachers { get; set; }

    public decimal AveragePerformance { get; set; }

    public int CompletedClasses { get; set; }


    public TeacherPerformanceDto? BestPerformingTeacher { get; set; }


    public List<PerformanceTrendDto> PerformanceTrend { get; set; }
        = new();


    public List<JoiningTrendDto> JoiningTrend { get; set; }
        = new();


    public List<TeacherRankingDto> TeacherRanking { get; set; }
    = new();
    public List<TeacherWorkloadDto> TeacherWorkload { get; set; }
    = new();
    public List<ProgramTeacherDistributionDto> ProgramDistribution { get; set; }
    = new();
}