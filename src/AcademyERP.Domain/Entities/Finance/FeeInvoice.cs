using AcademyERP.Domain.Entities.Common;
using AcademyERP.Domain.Entities.Enrollments;
using AcademyERP.Domain.Enums;

namespace AcademyERP.Domain.Entities.Finance;

public class FeeInvoice : BaseEntity
{
    public Guid EnrollmentId { get; set; }

    public Enrollment Enrollment { get; set; } = null!;

    public string InvoiceNumber { get; set; } = string.Empty;

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