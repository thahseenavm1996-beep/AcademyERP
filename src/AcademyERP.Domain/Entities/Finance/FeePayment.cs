using AcademyERP.Domain.Entities.Common;

namespace AcademyERP.Domain.Entities.Finance;

public class FeePayment : BaseEntity
{
    public Guid FeeInvoiceId { get; set; }

    public FeeInvoice FeeInvoice { get; set; } = null!;

    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; }

    public Guid PaymentMethodId { get; set; }

    public PaymentMethod PaymentMethod { get; set; } = null!;

    public string? TransactionReference { get; set; }

    public string? Remarks { get; set; }
}