namespace AcademyERP.Admin.Models;

public class UpdateFeeInvoiceRequest
{
    public int Status { get; set; }

    public DateTime? PaidDate { get; set; }

    public DateTime DueDate { get; set; }

    public string? Remarks { get; set; }
}