namespace AcademyERP.Application.DTOs.ParentPortal;

public class ParentProgressDto
{
    public Guid Id { get; set; }
    public Guid ScheduledClassId { get; set; }
    public Guid StudentId { get; set; }

    public string StudentName { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public string TeacherName { get; set; } = string.Empty;

    public string ClassDate { get; set; } = string.Empty;

    public int PerformanceRating { get; set; }
    public string HomeworkStatus { get; set; } = string.Empty;
    public int BehaviourRating { get; set; }

    public string ClassOutcome { get; set; } = string.Empty;
    public string LessonTaken { get; set; } = string.Empty;
    public string NextHomework { get; set; } = string.Empty;
    public string TeacherRemarks { get; set; } = string.Empty;

    public int ActualDurationMinutes { get; set; }
    public string SubmittedAt { get; set; } = string.Empty;

    public string AttendanceStatus { get; set; } = "Not Marked";
}
