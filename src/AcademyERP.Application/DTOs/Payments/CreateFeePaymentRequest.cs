namespace AcademyERP.Application.DTOs.Payments;

public class CreateFeePaymentRequest
{
    public Guid FeeInvoiceId { get; set; }

    public Guid PaymentMethodId { get; set; }

    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; }

    public string? TransactionReference { get; set; }

    public string? Remarks { get; set; }
}