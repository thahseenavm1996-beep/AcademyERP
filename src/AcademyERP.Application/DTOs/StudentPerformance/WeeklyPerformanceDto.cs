namespace AcademyERP.Application.DTOs.StudentPerformance;

public class WeeklyPerformanceDto
{
    public string Week { get; set; } = string.Empty;

    public double AttendancePercentage { get; set; }

    public double LearningScore { get; set; }

    public double HomeworkPercentage { get; set; }

    public double OverallScore { get; set; }
}