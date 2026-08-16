using AcademyERP.Domain.Entities.Common;
using AcademyERP.Domain.Enums;
using AcademyERP.Domain.Entities.Programs;
using AcademyERP.Domain.Entities.Courses;
using AcademyERP.Domain.Entities.Teachers;
using AcademyERP.Domain.Entities.Lookups;

namespace AcademyERP.Domain.Entities.Admissions;

public class RegistrationRequestStudent : BaseEntity
{
    public Guid RegistrationRequestId { get; set; }

    public RegistrationRequest RegistrationRequest
        { get; set; } = default!;

    public string StudentName { get; set; }
        = string.Empty;

    public DateTime DateOfBirth { get; set; }

    public Gender Gender { get; set; }

    public Guid ProgramId { get; set; }
    public Program Program { get; set; } = default!;

    public string? PreferredTime { get; set; }

    public string? Remarks { get; set; }
    public Guid? CourseId { get; set; }
public Course? Course { get; set; }

public Guid? TeacherId { get; set; }
public Teacher? Teacher { get; set; }

public Guid? TimeSlotId { get; set; }
public TimeSlot? TimeSlot { get; set; }

public Guid? ClassDurationId { get; set; }
public ClassDuration? ClassDuration { get; set; }
}