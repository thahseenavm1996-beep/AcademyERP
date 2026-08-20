namespace AcademyERP.Admin.Models.FinanceReports;

public class OutstandingFeeDto
{
    public string StudentName { get; set; } = string.Empty;

    public string CourseName { get; set; } = string.Empty;

    public decimal AmountDue { get; set; }

    public DateOnly DueDate { get; set; }

    public string Status { get; set; } = string.Empty;
}