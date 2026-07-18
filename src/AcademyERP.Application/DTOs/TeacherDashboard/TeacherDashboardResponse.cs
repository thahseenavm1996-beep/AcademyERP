namespace AcademyERP.Application.DTOs.TeacherDashboard;

public class TeacherDashboardResponse
{
    public TeacherSummaryResponse Summary { get; set; } = new();

    public List<TodayClassResponse> TodayClasses { get; set; } = new();
}