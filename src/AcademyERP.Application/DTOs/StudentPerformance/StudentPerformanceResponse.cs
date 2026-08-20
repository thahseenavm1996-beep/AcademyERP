namespace AcademyERP.Application.DTOs.StudentPerformance;

public class StudentPerformanceResponse
{
    public string StudentName { get; set; } = string.Empty;


    public int TotalClasses { get; set; }

    public int CompletedClasses { get; set; }

    public decimal AttendancePercentage { get; set; }


    public decimal LearningScore { get; set; }

    public decimal HomeworkPercentage { get; set; }


    public string Strength { get; set; } = string.Empty;

    public string ImprovementArea { get; set; } = string.Empty;

public int ConductedClasses { get; set; }

public int AttendedClasses { get; set; }
    public List<StudentTrendResponse> MonthlyTrend { get; set; }
        = new();

public List<WeeklyPerformanceDto> WeeklyTrend { get; set; }
    = new();
    public List<RecentProgressResponse> RecentProgress { get; set; }
        = new();


    public List<string> TeacherFeedback { get; set; }
        = new();
}