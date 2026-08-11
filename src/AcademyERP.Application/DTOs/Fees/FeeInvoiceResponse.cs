using AcademyERP.Domain.Enums;

namespace AcademyERP.Application.DTOs.Fees;

public class FeeInvoiceResponse
{
    public Guid Id { get; set; }

    public string InvoiceNumber { get; set; }
        = string.Empty;

    public Guid EnrollmentId { get; set; }

    public string StudentName { get; set; }
        = string.Empty;

    public string CourseName { get; set; }
        = string.Empty;

    public decimal Amount { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal PaidAmount { get; set; }

    public decimal BalanceAmount { get; set; }

    public DateTime InvoiceDate { get; set; }

    public DateTime DueDate { get; set; }

    public DateTime? PaidDate { get; set; }

    public FeeStatus Status { get; set; }

    public string? Remarks { get; set; }
}