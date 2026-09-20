namespace AcademyERP.Admin.Models.TeacherPayments;

public class UpdateTeacherPaymentConfigurationRequest
{
    public decimal PaymentAmount { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public string? Remarks { get; set; }
}
