using AcademyERP.Domain.Entities.Common;
using AcademyERP.Domain.Entities.ScheduledClasses;
using AcademyERP.Domain.Entities.Teachers;
using AcademyERP.Domain.Enums;



namespace AcademyERP.Domain.Entities.ReportUnlockRequests;

public class ReportUnlockRequest : BaseEntity
{
    public Guid ScheduledClassId { get; set; }

    public ScheduledClass ScheduledClass { get; set; } = null!;

    public Guid TeacherId { get; set; }

    public Teacher Teacher { get; set; } = null!;

    public DateOnly ReportDate { get; set; }

    public string Reason { get; set; } = string.Empty;

    public ReportUnlockRequestStatus Status { get; set; }
        = ReportUnlockRequestStatus.Pending;

    public DateTime RequestedAt { get; set; }
        = DateTime.UtcNow;

    public DateTime? ReviewedAt { get; set; }

    public Guid? ReviewedByUserId { get; set; }

    public string? AdminRemarks { get; set; }
}
