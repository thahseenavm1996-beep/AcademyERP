namespace AcademyERP.Application.DTOs.ParentPortal;

public class ParentAttendanceResponse
{
    public ParentAttendanceSummaryDto Summary { get; set; } = new();
    public List<ParentAttendanceDto> Records { get; set; } = new();
}
