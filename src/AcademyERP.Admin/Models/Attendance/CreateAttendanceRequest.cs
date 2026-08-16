using AcademyERP.Domain.Enums;

namespace AcademyERP.Admin.Models.Attendance;

public class CreateAttendanceRequest
{
    public Guid EnrollmentId { get; set; }
    public DateTime AttendanceDate { get; set; } = DateTime.Today;
    public AttendanceStatus Status { get; set; } = AttendanceStatus.Present;
    public string? Remarks { get; set; }
}
