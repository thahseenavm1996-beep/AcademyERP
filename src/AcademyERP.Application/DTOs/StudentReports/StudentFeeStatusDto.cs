namespace AcademyERP.Application.DTOs.StudentReports;

public class StudentFeeStatusDto
{
    public string StudentName { get; set; } = "";

    public string CourseName { get; set; } = "";

    public decimal MonthlyFee { get; set; }

    public DateOnly LastPaymentDate { get; set; }

    public DateOnly NextDueDate { get; set; }

    public string Status { get; set; } = "";
}