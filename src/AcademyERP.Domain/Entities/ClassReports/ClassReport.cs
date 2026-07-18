using AcademyERP.Domain.Entities.Common;
using AcademyERP.Domain.Entities.Enrollments;
using AcademyERP.Domain.Entities.Students;
using AcademyERP.Domain.Entities.Teachers;
using AcademyERP.Domain.Enums;

namespace AcademyERP.Domain.Entities.ClassReports;

public class ClassReport : BaseEntity
{
    public Guid TeacherId { get; set; }

    public Teacher Teacher { get; set; } = null!;

    public Guid StudentId { get; set; }

    public Student Student { get; set; } = null!;

    public Guid EnrollmentId { get; set; }

    public Enrollment Enrollment { get; set; } = null!;

    public DateTime ReportDate { get; set; }

    public AttendanceStatus AttendanceStatus { get; set; }

    public PerformanceRating PerformanceRating { get; set; }

    public HomeworkStatus HomeworkStatus { get; set; }

    public BehaviourRating BehaviourRating { get; set; }

    public ClassOutcome ClassOutcome { get; set; }

    public string LessonTaken { get; set; } = string.Empty;

    public string NextHomework { get; set; } = string.Empty;

    public string? TeacherRemarks { get; set; }

    public int ActualDurationMinutes { get; set; }

    public DateTime SubmittedAt { get; set; }

    public bool IsLocked { get; set; } = false;
}