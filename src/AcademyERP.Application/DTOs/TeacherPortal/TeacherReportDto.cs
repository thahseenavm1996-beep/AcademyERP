using AcademyERP.Domain.Enums;

namespace AcademyERP.Application.DTOs.TeacherPortal;

public class TeacherReportDto
{
    public Guid ScheduledClassId { get; set; }
public AttendanceStatus AttendanceStatus { get; set; } = AttendanceStatus.Present;

    public string StudentName { get; set; } = "";

    public DateOnly ClassDate { get; set; }

    public string LessonTaken { get; set; } = "";

    public string NextHomework { get; set; } = "";

    public string TeacherRemarks { get; set; } = "";

    public PerformanceRating PerformanceRating { get; set; }

    public HomeworkStatus HomeworkStatus { get; set; }

    public BehaviourRating BehaviourRating { get; set; }

    public ClassOutcome ClassOutcome { get; set; }

    public int ActualDurationMinutes { get; set; }
}
