namespace AcademyERP.Admin.Models.StudentPerformance;

public class RecentProgressResponse
{
    public DateOnly Date { get; set; }

    public string CourseName { get; set; } = string.Empty;

    public string LessonTitle { get; set; } = string.Empty;

    public bool Completed { get; set; }
}