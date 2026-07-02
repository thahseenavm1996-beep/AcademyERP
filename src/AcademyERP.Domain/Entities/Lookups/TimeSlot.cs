using AcademyERP.Domain.Entities.Common;
using AcademyERP.Domain.Entities.TeacherAvailabilities;

namespace AcademyERP.Domain.Entities.Lookups;

public class TimeSlot : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public bool IsActive { get; set; } = true;
    public ICollection<TeacherAvailability> TeacherAvailabilities { get; set; } = new List<TeacherAvailability>();
}