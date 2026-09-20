namespace AcademyERP.Admin.Models.ParentPortal;

public class ParentFeesResponse
{
    public decimal TotalInvoiced { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal OutstandingAmount { get; set; }
    public decimal CreditAmount { get; set; }

    public List<ParentFeeInvoiceDto> Invoices { get; set; } = new();
    public List<ParentFeePaymentDto> Payments { get; set; } = new();
}
