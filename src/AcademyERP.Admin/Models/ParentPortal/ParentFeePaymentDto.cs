namespace AcademyERP.Admin.Models.ParentPortal;

public class ParentFeePaymentDto
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string PaymentDate { get; set; } = string.Empty;
    public string PaymentMethodName { get; set; } = string.Empty;
    public string? TransactionReference { get; set; }
    public string? Remarks { get; set; }
}
