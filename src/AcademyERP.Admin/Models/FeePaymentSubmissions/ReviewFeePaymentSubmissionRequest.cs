namespace AcademyERP.Admin.Models.FeePaymentSubmissions;

public class ReviewFeePaymentSubmissionRequest
{
    public bool Approve { get; set; }

    public string? RejectionReason { get; set; }
}