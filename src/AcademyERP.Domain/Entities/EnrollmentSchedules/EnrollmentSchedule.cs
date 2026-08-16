using AcademyERP.Domain.Entities.Common;
using AcademyERP.Domain.Entities.Enrollments;
using AcademyERP.Domain.Entities.Lookups;

namespace AcademyERP.Domain.Entities.EnrollmentSchedules;

public class EnrollmentSchedule : BaseEntity
{
    public Guid EnrollmentId { get; set; }

    public Enrollment Enrollment { get; set; } = null!;


    public DayOfWeek DayOfWeek { get; set; }


    public Guid TimeSlotId { get; set; }

    public TimeSlot TimeSlot { get; set; } = null!;


    public Guid ClassDurationId { get; set; }

    public ClassDuration ClassDuration { get; set; } = null!;


    public bool IsActive { get; set; } = true;
}