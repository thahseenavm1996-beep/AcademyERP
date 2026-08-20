using AcademyERP.Domain.Entities.Common;
using AcademyERP.Domain.Entities.Enrollments;
using AcademyERP.Domain.Entities.Students;
using AcademyERP.Domain.Entities.Teachers;
using AcademyERP.Domain.Enums;

using AcademyERP.Domain.Entities.ScheduledClasses;
namespace AcademyERP.Domain.Entities.ClassReports;
public class ClassReport : BaseEntity
{
    public Guid ScheduledClassId { get; set; }

    public ScheduledClass ScheduledClass { get; set; } = null!;


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