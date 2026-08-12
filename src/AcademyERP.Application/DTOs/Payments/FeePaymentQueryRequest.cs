namespace AcademyERP.Application.DTOs.Payments;

public class FeePaymentQueryRequest
{
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;

    public Guid? FeeInvoiceId { get; set; }
}