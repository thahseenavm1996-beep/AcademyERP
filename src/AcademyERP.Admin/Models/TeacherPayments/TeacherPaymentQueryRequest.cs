namespace AcademyERP.Admin.Models.TeacherPayments;

public class TeacherPaymentQueryRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;

    public Guid? TeacherId { get; set; }
    public Guid? EnrollmentId { get; set; }

    public string? TeacherName { get; set; }
    public string? StudentName { get; set; }

    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
