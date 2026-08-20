namespace AcademyERP.Admin.Models.StudentReports;

public class LearningProgressSummaryResponse
{
    public int TotalLessons { get; set; }

    public int CompletedLessons { get; set; }

    public int ChaptersCompleted { get; set; }

    public string? PagesCovered { get; set; }
}