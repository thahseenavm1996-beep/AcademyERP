namespace AcademyERP.Admin.Models.TeacherPortal;

public class TeacherStudentDto
{
    public Guid StudentId { get; set; }

    public string StudentName { get; set; } = "";

    public string CourseName { get; set; } = "";

    public string ParentName { get; set; } = "";

    public string Status { get; set; } = "";

    public int AttendancePercentage { get; set; }
}