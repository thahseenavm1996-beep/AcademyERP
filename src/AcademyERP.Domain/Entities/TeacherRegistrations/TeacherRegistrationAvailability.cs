using AcademyERP.Domain.Entities.Common;
using AcademyERP.Domain.Entities.Lookups;

namespace AcademyERP.Domain.Entities.TeacherRegistrations;

public class TeacherRegistrationAvailability : BaseEntity
{
    public Guid TeacherRegistrationRequestId { get; set; }

    public TeacherRegistrationRequest TeacherRegistrationRequest { get; set; } = null!;

    public DayOfWeek DayOfWeek { get; set; }

    public Guid TimeSlotId { get; set; }

    public TimeSlot TimeSlot { get; set; } = null!;
}
