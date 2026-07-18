using AcademyERP.Domain.Entities.Common;
using AcademyERP.Domain.Entities.TeachingSchedules;
using AcademyERP.Domain.Enums;

namespace AcademyERP.Domain.Entities.ScheduledClasses;

public class ScheduledClass : BaseEntity
{
    // Generated from the recurring teaching schedule
    public Guid TeachingScheduleId { get; set; }

    public TeachingSchedule TeachingSchedule { get; set; } = null!;

    // Actual class date
    public DateOnly ClassDate { get; set; }

    // Scheduled start time
    public TimeOnly StartTime { get; set; }

    // Scheduled end time
    public TimeOnly EndTime { get; set; }

    // Current status
    public ScheduledClassStatus Status { get; set; }
        = ScheduledClassStatus.Scheduled;

    // Teacher actually started the class
    public DateTime? ActualStartTime { get; set; }

    // Teacher finished the class
    public DateTime? ActualEndTime { get; set; }

    // Holiday / Teacher Leave / Student Leave etc.
    public string? CancellationReason { get; set; }

    // Admin notes
    public string? Remarks { get; set; }
}