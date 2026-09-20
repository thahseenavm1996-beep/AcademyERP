namespace AcademyERP.Application.DTOs.ParentPortal;

public class ParentFeeInvoiceDto
{
    public Guid Id { get; set; }
    public Guid EnrollmentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public string InvoiceNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal BalanceAmount { get; set; }
    public string InvoiceDate { get; set; } = string.Empty;
    public string DueDate { get; set; } = string.Empty;
    public string? PaidDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Remarks { get; set; }
}
