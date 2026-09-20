using AcademyERP.Domain.Enums;

namespace AcademyERP.Admin.Models.FeePaymentSubmissions;

public class FeePaymentSubmissionResponse
{
    public Guid Id { get; set; }

    public Guid FeeInvoiceId { get; set; }

    public string InvoiceNumber { get; set; } = string.Empty;

    public string StudentName { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;

public string Gender { get; set; } = string.Empty;

    public Guid TeacherId { get; set; }

    public string TeacherName { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; }

    public Guid PaymentMethodId { get; set; }

    public string PaymentMethodName { get; set; } = string.Empty;

    public string? TransactionReference { get; set; }

    public string? ProofFilePath { get; set; }

    public string? PaymentProofFileName { get; set; }

public DateTime? PaymentProofUploadedOn { get; set; }

    public string? Remarks { get; set; }

    public FeePaymentSubmissionStatus Status { get; set; }

    public DateTime SubmittedAt { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public Guid? ReviewedBy { get; set; }

    public string? RejectionReason { get; set; }
}