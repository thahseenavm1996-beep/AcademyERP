using AcademyERP.Domain.Entities.Common;

namespace AcademyERP.Domain.Entities.Enrollments;

/// <summary>
/// A temporary leave which extends, rather than skips, the next fee cycle.
/// ResumeDate is the first day the student is back in class.
/// </summary>
public class EnrollmentFeePause : BaseEntity
{
    public Guid EnrollmentId { get; set; }

    public Enrollment Enrollment { get; set; } = null!;

    public DateTime PauseStartDate { get; set; }

    public DateTime? ResumeDate { get; set; }

    public string? Reason { get; set; }
}
