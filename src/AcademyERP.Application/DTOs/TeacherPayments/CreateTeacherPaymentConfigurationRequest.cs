namespace AcademyERP.Application.DTOs.TeacherPayments;

public class CreateTeacherPaymentConfigurationRequest
{
    public Guid TeacherId { get; set; }
    public Guid EnrollmentId { get; set; }
    public decimal PaymentAmount { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public string? Remarks { get; set; }
}
