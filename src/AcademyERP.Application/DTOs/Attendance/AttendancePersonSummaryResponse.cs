namespace AcademyERP.Application.DTOs.Attendance;

public class AttendancePersonSummaryResponse : AttendanceSummaryResponse
{
    public Guid PersonId { get; set; }
    public string PersonName { get; set; } = string.Empty;
}
