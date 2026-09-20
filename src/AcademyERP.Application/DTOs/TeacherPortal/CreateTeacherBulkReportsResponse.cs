namespace AcademyERP.Application.DTOs.TeacherPortal;

public class CreateTeacherBulkReportsResponse
{
    public int CreatedCount { get; set; }
    public int SkippedCount { get; set; }
    public List<Guid> CreatedReportIds { get; set; } = new();
    public List<Guid> SkippedScheduledClassIds { get; set; } = new();
}
