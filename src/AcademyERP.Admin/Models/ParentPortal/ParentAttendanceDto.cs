namespace AcademyERP.Admin.Models.ParentPortal;

public class ParentAttendanceDto
{
    public Guid Id { get; set; }
    public Guid ScheduledClassId { get; set; }
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public string TeacherName { get; set; } = string.Empty;
    public string ClassDate { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Remarks { get; set; }
}
