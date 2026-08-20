namespace AcademyERP.Admin.Models.StudentReports;

public class StudentLearningProgressDto
{
    public string StudentName { get; set; } = "";

    public string CourseName { get; set; } = "";

    public int LessonsCompleted { get; set; }

    public int PagesCovered { get; set; }

    public decimal ProgressPercentage { get; set; }
}