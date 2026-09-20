namespace AcademyERP.Application.DTOs.ReportUnlockRequests;

public class CreateReportUnlockRequest
{
    public Guid ScheduledClassId { get; set; }

    public string Reason { get; set; } = string.Empty;
}