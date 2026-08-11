using AcademyERP.Domain.Enums;

namespace AcademyERP.Application.DTOs.Fees;

public class FeeInvoiceQueryRequest
{
    public FeeStatus? Status { get; set; }

    public Guid? StudentId { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;
}