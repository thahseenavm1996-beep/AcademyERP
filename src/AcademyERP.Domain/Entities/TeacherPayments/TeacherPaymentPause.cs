using AcademyERP.Domain.Entities.Common;

namespace AcademyERP.Domain.Entities.TeacherPayments;

public class TeacherPaymentPause : BaseEntity
{
    public Guid TeacherPaymentConfigurationId { get; set; }

    public TeacherPaymentConfiguration TeacherPaymentConfiguration { get; set; } = null!;

    /// <summary>
    /// First day on which salary generation is paused.
    /// </summary>
    public DateTime PauseStartDate { get; set; }

    /// <summary>
    /// First day on which salary generation resumes.
    /// Null means the pause is currently active.
    /// </summary>
    public DateTime? ResumeDate { get; set; }

    public string? Reason { get; set; }
}