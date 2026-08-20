namespace AcademyERP.Application.DTOs.Students;

public class StudentProfileSummaryResponse
{
    public int ProgramsCount { get; set; }

    public int TeachersCount { get; set; }

    public decimal AttendancePercentage { get; set; }

    public decimal OutstandingFees { get; set; }
}