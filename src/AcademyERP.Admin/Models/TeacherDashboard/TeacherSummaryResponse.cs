namespace AcademyERP.Admin.Models.TeacherDashboard;

public class TeacherSummaryResponse
{
    public int TotalClassesToday { get; set; }

    public int CompletedClasses { get; set; }

    public int PendingClasses { get; set; }

    public int MissedClasses { get; set; }

    public int CompletionPercentage { get; set; }
}