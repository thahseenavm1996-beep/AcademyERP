using AcademyERP.Domain.Entities.Common;
using AcademyERP.Domain.Entities.ScheduledClasses;

namespace AcademyERP.Domain.Entities.AcademicProgress;

public class ClassProgress : BaseEntity
{
    public Guid ScheduledClassId { get; set; }

    public ScheduledClass ScheduledClass { get; set; } = null!;


    public string? LessonTitle { get; set; }

    public string? ChapterName { get; set; }


    public string? PagesCovered { get; set; }


    public bool IsLessonCompleted { get; set; }


    public string? TeacherNotes { get; set; }


    public string? Homework { get; set; }


    public bool HomeworkGiven { get; set; }


    public bool HomeworkCompleted { get; set; }
}