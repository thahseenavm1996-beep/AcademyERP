namespace AcademyERP.Application.DTOs.TeacherPortal;

public class TeacherClassDto
{
    public Guid Id { get; set; }

    public DateOnly ClassDate { get; set; }

    public string StudentName { get; set; } = "";

    public string CourseName { get; set; } = "";

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public string Status { get; set; } = "";
    public bool AttendanceMarked { get; set; }
}