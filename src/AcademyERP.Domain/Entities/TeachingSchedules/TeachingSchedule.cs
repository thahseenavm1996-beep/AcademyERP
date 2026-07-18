using AcademyERP.Domain.Entities.Common;
using AcademyERP.Domain.Entities.Enrollments;
using AcademyERP.Domain.Entities.Lookups;
using AcademyERP.Domain.Entities.Teachers;
using AcademyERP.Domain.Entities.ScheduledClasses;

namespace AcademyERP.Domain.Entities.TeachingSchedules;

public class TeachingSchedule : BaseEntity
{
    // One Enrollment = One Teaching Schedule
    public Guid EnrollmentId { get; set; }

    public Enrollment Enrollment { get; set; } = null!;

    // Assigned Teacher
    public Guid TeacherId { get; set; }

    public Teacher Teacher { get; set; } = null!;

    // Weekly Schedule
    public DayOfWeek DayOfWeek { get; set; }

    // Example:
    // 09:00
    // 18:30
    public TimeOnly StartTime { get; set; }

    // Duration Lookup
    public Guid ClassDurationId { get; set; }

    public ClassDuration ClassDuration { get; set; } = null!;

    // Schedule Validity
    public DateTime EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }

    // Active / Inactive
    public bool IsActive { get; set; } = true;
    public int MaximumStudents { get; set; } = 1;
    public ICollection<ScheduledClass> ScheduledClasses { get; set; }
          = new List<ScheduledClass>();
    // Optional Notes
    public string? Remarks { get; set; }

}