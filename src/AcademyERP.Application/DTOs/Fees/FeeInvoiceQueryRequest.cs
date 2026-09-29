using AcademyERP.Domain.Enums;

namespace AcademyERP.Application.DTOs.Fees;

public class FeeInvoiceQueryRequest
{
    public FeeStatus? Status { get; set; }

    public Guid? StudentId { get; set; }

    public string? StudentName { get; set; }

    public string? TeacherName { get; set; }

    public Guid? TeacherId { get; set; }

    public Guid? CourseId { get; set; }

    public string? Gender { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;
}