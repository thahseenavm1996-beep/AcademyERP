using AcademyERP.Domain.Entities.Common;

namespace AcademyERP.Domain.Entities.Lookups;

public class TimeSlot : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public bool IsActive { get; set; } = true;
}