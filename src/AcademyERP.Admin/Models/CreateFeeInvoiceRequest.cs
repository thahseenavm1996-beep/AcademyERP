namespace AcademyERP.Admin.Models;

public class CreateFeeInvoiceRequest
{
    public Guid EnrollmentId { get; set; }

    public DateTime InvoiceDate { get; set; }

    public DateTime DueDate { get; set; }

    public string? Remarks { get; set; }
}