using AcademyERP.Domain.Entities.Common;
using AcademyERP.Domain.Entities.Enrollments;
using AcademyERP.Domain.Entities.Students;
using AcademyERP.Domain.Entities.Teachers;
using AcademyERP.Domain.Enums;
using AcademyERP.Domain.Entities.ScheduledClasses;

namespace AcademyERP.Domain.Entities.Attendance;

public class Attendance : BaseEntity
{
    public Guid ScheduledClassId { get; set; }

    public ScheduledClass ScheduledClass { get; set; } = null!;


    public AttendanceStatus Status { get; set; }


    public string? Remarks { get; set; }

}