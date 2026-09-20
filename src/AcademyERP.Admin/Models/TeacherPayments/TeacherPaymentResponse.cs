namespace AcademyERP.Admin.Models.TeacherPayments;

public class TeacherPaymentResponse
{
    public Guid Id { get; set; }

    public Guid TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;

    public Guid EnrollmentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;

    public DateTime CycleStartDate { get; set; }
    public DateTime CycleEndDate { get; set; }
    public DateTime DueDate { get; set; }

    public decimal Amount { get; set; }

    public int Status { get; set; }

    public DateTime? PaidDate { get; set; }

    public Guid? PaymentMethodId { get; set; }

    public string? TransactionReference { get; set; }
    public string? Remarks { get; set; }
    

    public DateTime PayableFromDate { get; set; }

public DateTime PayableToDate { get; set; }

public int PayableDays { get; set; }

public int CycleScheduledDays { get; set; }
}
