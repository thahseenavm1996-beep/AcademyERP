using AcademyERP.Domain.Entities.Common;
using AcademyERP.Domain.Entities.Courses;
using AcademyERP.Domain.Entities.Lookups;
using AcademyERP.Domain.Entities.Students;

namespace AcademyERP.Domain.Entities.TeacherRegistrations;

public class TeacherRegistrationClass : BaseEntity
{
    public Guid TeacherRegistrationRequestId { get; set; }

    public TeacherRegistrationRequest TeacherRegistrationRequest { get; set; } = null!;

  public Guid? StudentId { get; set; }

public Student? Student { get; set; }

public string? StudentName { get; set; }

    public Guid CourseId { get; set; }

    public Course Course { get; set; } = null!;

    public DayOfWeek DayOfWeek { get; set; }

    public Guid TimeSlotId { get; set; }

    public TimeSlot TimeSlot { get; set; } = null!;

    public Guid ClassDurationId { get; set; }

    public ClassDuration ClassDuration { get; set; } = null!;

    public string? ClassType { get; set; }
}
