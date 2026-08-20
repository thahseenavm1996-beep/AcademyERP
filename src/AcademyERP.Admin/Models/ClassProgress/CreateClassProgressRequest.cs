namespace AcademyERP.Admin.Models.ClassProgress;

public class CreateClassProgressRequest
{
    public Guid ScheduledClassId { get; set; }

    public string? LessonTitle { get; set; }

    public string? ChapterName { get; set; }

    public string? PagesCovered { get; set; }

    public bool IsLessonCompleted { get; set; }

    public string? TeacherNotes { get; set; }

    public string? Homework { get; set; }

    public bool HomeworkGiven { get; set; }

    public bool HomeworkCompleted { get; set; }
}