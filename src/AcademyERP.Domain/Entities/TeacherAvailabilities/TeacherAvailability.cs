using AcademyERP.Domain.Entities.Common;
using AcademyERP.Domain.Entities.Lookups;
using AcademyERP.Domain.Entities.Teachers;

namespace AcademyERP.Domain.Entities.TeacherAvailabilities;

public class TeacherAvailability : BaseEntity
{
    public Guid TeacherId { get; set; }

    public DayOfWeek DayOfWeek { get; set; }

    public Guid TimeSlotId { get; set; }

    public bool IsAvailable { get; set; } = true;

    // Navigation Properties
    public Teacher Teacher { get; set; } = null!;

    public TimeSlot TimeSlot { get; set; } = null!;
}