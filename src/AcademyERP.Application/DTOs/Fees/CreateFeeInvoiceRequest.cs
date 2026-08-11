using System.ComponentModel.DataAnnotations;

namespace AcademyERP.Application.DTOs.Fees;

public class CreateFeeInvoiceRequest
{
    [Required]
    public Guid EnrollmentId { get; set; }

    [Required]
    public DateTime InvoiceDate { get; set; }
        = DateTime.Today;

    [Required]
    public DateTime DueDate { get; set; }
        = DateTime.Today.AddDays(7);

    public string? Remarks { get; set; }
}