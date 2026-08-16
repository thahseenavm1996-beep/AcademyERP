using AcademyERP.Domain.Enums;

namespace AcademyERP.Admin.Models.Attendance;

public class UpdateAttendanceRequest
{
    public AttendanceStatus Status { get; set; }
    public string? Remarks { get; set; }
}
