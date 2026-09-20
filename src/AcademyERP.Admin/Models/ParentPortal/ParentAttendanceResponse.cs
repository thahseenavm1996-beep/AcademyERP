namespace AcademyERP.Admin.Models.ParentPortal;

public class ParentAttendanceResponse
{
    public ParentAttendanceSummaryDto Summary { get; set; } = new();
    public List<ParentAttendanceDto> Records { get; set; } = new();
}
