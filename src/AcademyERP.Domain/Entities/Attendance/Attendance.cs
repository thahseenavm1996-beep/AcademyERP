using AcademyERP.Domain.Entities.Common;
using AcademyERP.Domain.Entities.Enrollments;
using AcademyERP.Domain.Entities.Students;
using AcademyERP.Domain.Entities.Teachers;
using AcademyERP.Domain.Enums;

namespace AcademyERP.Domain.Entities.Attendance;

public class Attendance : BaseEntity
{
    public Guid EnrollmentId { get; set; }

    public Enrollment Enrollment { get; set; } = null!;

    public DateTime AttendanceDate { get; set; }

    public AttendanceStatus Status { get; set; }

    public string? Remarks { get; set; }
}