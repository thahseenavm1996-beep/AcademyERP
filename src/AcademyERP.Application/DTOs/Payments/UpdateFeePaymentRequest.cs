namespace AcademyERP.Application.DTOs.Payments;

public class UpdateFeePaymentRequest
{
    public Guid PaymentMethodId { get; set; }

    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; }

    public string? TransactionReference { get; set; }

    public string? Remarks { get; set; }
}