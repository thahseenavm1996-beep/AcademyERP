using AcademyERP.Domain.Enums;

namespace AcademyERP.Application.DTOs.Fees;

public class UpdateFeeInvoiceRequest
{
    

    public FeeStatus Status { get; set; }

    public DateTime? PaidDate { get; set; }

    public DateTime DueDate { get; set; }

    public string? Remarks { get; set; }
}