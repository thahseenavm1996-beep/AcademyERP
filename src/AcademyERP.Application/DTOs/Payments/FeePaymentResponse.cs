namespace AcademyERP.Application.DTOs.Payments;

public class FeePaymentResponse
{
    public Guid Id { get; set; }

    public Guid FeeInvoiceId { get; set; }

    public string InvoiceNumber { get; set; } = string.Empty;

    public string StudentName { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; }

    public Guid PaymentMethodId { get; set; }

    public string PaymentMethodName { get; set; } = string.Empty;

    public string? TransactionReference { get; set; }

    public string? Remarks { get; set; }
}