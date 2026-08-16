namespace AcademyERP.Admin.Models.Registration;

public class RegistrationStudentResponse
{
    public Guid Id { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public DateTime DateOfBirth { get; set; }

    public int Gender { get; set; }

    public Guid ProgramId { get; set; }

    public string ProgramName { get; set; } = string.Empty;

    public string? PreferredTime { get; set; }

    // NEW

    public Guid? CourseId { get; set; }
    public string? CourseName { get; set; }

    public Guid? TeacherId { get; set; }
    public string? TeacherName { get; set; }

    public Guid? TimeSlotId { get; set; }
    public string? TimeSlotName { get; set; }

    public Guid? ClassDurationId { get; set; }
    public string? ClassDurationName { get; set; }
}