using AcademyERP.Domain.Enums;

namespace AcademyERP.Application.DTOs.ClassReports;

public class ClassReportResponse
{
    public Guid Id { get; set; }

    public Guid ScheduledClassId { get; set; }


    public Guid TeacherId { get; set; }

    public string TeacherName { get; set; } = string.Empty;


    public Guid StudentId { get; set; }

    public string StudentName { get; set; } = string.Empty;


    public string CourseName { get; set; } = string.Empty;


    public DateOnly ClassDate { get; set; }


    public PerformanceRating PerformanceRating { get; set; }

    public HomeworkStatus HomeworkStatus { get; set; }

    public BehaviourRating BehaviourRating { get; set; }

    public ClassOutcome ClassOutcome { get; set; }


    public string LessonTaken { get; set; } = string.Empty;

    public string NextHomework { get; set; } = string.Empty;

    public string? TeacherRemarks { get; set; }


    public int ActualDurationMinutes { get; set; }


    public DateTime SubmittedAt { get; set; }


    public bool IsLocked { get; set; }
}