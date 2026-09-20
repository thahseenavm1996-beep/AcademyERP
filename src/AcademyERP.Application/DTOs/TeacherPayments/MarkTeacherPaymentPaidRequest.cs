namespace AcademyERP.Application.DTOs.TeacherPayments;

public class MarkTeacherPaymentPaidRequest
{
    public DateTime PaidDate { get; set; }

    public Guid? PaymentMethodId { get; set; }

    public string? TransactionReference { get; set; }

    public string? Remarks { get; set; }
}
