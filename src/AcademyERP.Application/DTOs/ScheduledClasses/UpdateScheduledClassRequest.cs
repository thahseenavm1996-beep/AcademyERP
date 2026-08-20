using AcademyERP.Domain.Enums;

namespace AcademyERP.Application.DTOs.ScheduledClasses;

public class UpdateScheduledClassRequest
{
    public ScheduledClassStatus Status { get; set; }

    public string? CancellationReason { get; set; }

    public string? Remarks { get; set; }
}