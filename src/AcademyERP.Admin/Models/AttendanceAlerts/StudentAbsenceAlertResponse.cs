namespace AcademyERP.Admin.Models.AttendanceAlerts;

public class StudentAbsenceAlertResponse
{
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;

    public Guid? ParentId { get; set; }
    public string ParentName { get; set; } = string.Empty;
    public string ParentPhoneNumber { get; set; } = string.Empty;

    public Guid? TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;

    public string CourseName { get; set; } = string.Empty;

    public int ConsecutiveMissedClasses { get; set; }

    public DateOnly LastMissedDate { get; set; }

    public DateOnly? LastAttendedDate { get; set; }

    public string LastAttendanceStatus { get; set; } = string.Empty;

    public string AlertLevel { get; set; } = string.Empty;
}