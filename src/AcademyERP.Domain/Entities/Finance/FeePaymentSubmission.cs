using AcademyERP.Domain.Entities.Common;
using AcademyERP.Domain.Entities.Enrollments;
using AcademyERP.Domain.Entities.Finance;
using AcademyERP.Domain.Entities.Teachers;
using AcademyERP.Domain.Enums;

namespace AcademyERP.Domain.Entities.Finance;

public class FeePaymentSubmission : BaseEntity
{
    public Guid FeeInvoiceId { get; set; }
    public FeeInvoice FeeInvoice { get; set; } = null!;

    public Guid TeacherId { get; set; }
    public Teacher Teacher { get; set; } = null!;

    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; }

    public Guid PaymentMethodId { get; set; }
    public PaymentMethod PaymentMethod { get; set; } = null!;

    public string? TransactionReference { get; set; }

    public string? ProofFilePath { get; set; }

    public string? Remarks { get; set; }

    public FeePaymentSubmissionStatus Status { get; set; }
        = FeePaymentSubmissionStatus.Pending;

    public DateTime SubmittedAt { get; set; }
        = DateTime.UtcNow;

    public DateTime? ReviewedAt { get; set; }

    public Guid? ReviewedBy { get; set; }

    public string? RejectionReason { get; set; }
    public string? PaymentProofFileName { get; set; }

public string? PaymentProofPath { get; set; }

public DateTime? PaymentProofUploadedOn { get; set; }
}