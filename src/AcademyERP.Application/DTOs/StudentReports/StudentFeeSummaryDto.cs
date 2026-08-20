namespace AcademyERP.Application.DTOs.StudentReports;

public class StudentFeeSummaryDto
{
    public decimal TotalCollected { get; set; }

    public decimal PendingAmount { get; set; }

    public int PaidStudents { get; set; }

    public int PendingStudents { get; set; }
}