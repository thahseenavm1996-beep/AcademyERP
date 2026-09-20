namespace AcademyERP.Application.DTOs.FeePaymentSubmissions;

public class CreateFeePaymentSubmissionRequest
{
    public Guid FeeInvoiceId { get; set; }

    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; }

    public Guid PaymentMethodId { get; set; }

    public string? TransactionReference { get; set; }

    public string? ProofFilePath { get; set; }

    public string? Remarks { get; set; }
}