using AcademyERP.Domain.Enums;

namespace AcademyERP.Admin.Models.Attendance;

public class CreateAttendanceRequest
{
   
    public Guid ScheduledClassId { get; set; }

  
    public AttendanceStatus Status { get; set; } = AttendanceStatus.Present;

    
    public string? Remarks { get; set; }
}
