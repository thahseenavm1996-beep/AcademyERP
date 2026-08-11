namespace AcademyERP.Admin.Models;

public class DashboardSummaryResponse
{
    public int TotalStudents { get; set; }

    public int TotalTeachers { get; set; }

    public int TotalParents { get; set; }

    public int TotalPrograms { get; set; }

    public int ActiveStudents { get; set; }

    public int ActiveTeachers { get; set; }

    public List<RecentStudentDto> RecentStudents { get; set; }
        = new();
}