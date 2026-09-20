namespace AcademyERP.Application.DTOs.TeacherPortal;

public class TeacherAttendanceDto
{
    public DateOnly Date { get; set; }

    public string StudentName { get; set; } = "";

    public string Status { get; set; } = "";
}