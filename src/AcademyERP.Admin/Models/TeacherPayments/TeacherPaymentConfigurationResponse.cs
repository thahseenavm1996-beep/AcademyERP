namespace AcademyERP.Admin.Models.TeacherPayments;

public class TeacherPaymentConfigurationResponse
{
    public Guid Id { get; set; }
    public Guid TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;

    public Guid EnrollmentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;

    public decimal PaymentAmount { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }

    public bool IsPaused { get; set; }

public DateTime? PauseStartDate { get; set; }

public string? PauseReason { get; set; }
    public string? Remarks { get; set; }
}
