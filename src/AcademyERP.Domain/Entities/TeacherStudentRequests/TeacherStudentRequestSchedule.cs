using AcademyERP.Domain.Entities.Common;
using AcademyERP.Domain.Entities.Lookups;

namespace AcademyERP.Domain.Entities.TeacherStudentRequests;

public class TeacherStudentRequestSchedule : BaseEntity
{
    public Guid TeacherStudentRequestId { get; set; }

    public TeacherStudentRequest TeacherStudentRequest { get; set; }
        = null!;

    public DayOfWeek DayOfWeek { get; set; }

    public Guid TimeSlotId { get; set; }

    public TimeSlot TimeSlot { get; set; } = null!;

    public Guid ClassDurationId { get; set; }

    public ClassDuration ClassDuration { get; set; } = null!;
}