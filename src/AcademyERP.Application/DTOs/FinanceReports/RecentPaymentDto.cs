namespace AcademyERP.Application.DTOs.FinanceReports;

public class RecentPaymentDto
{
    public string StudentName { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public DateOnly PaymentDate { get; set; }

    public string PaymentMethod { get; set; } = string.Empty;
}