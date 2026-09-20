namespace AcademyERP.Application.DTOs.Dashboard;

public class ProgramPerformanceDto
{
    public Guid ProgramId { get; set; }

    public string ProgramName { get; set; } = string.Empty;

    public int EnrollmentCount { get; set; }

    public int AttendanceCount { get; set; }

    public int PresentCount { get; set; }

    public decimal AttendancePercentage { get; set; }

    public decimal AveragePerformance { get; set; }

    public int PerformanceReportCount { get; set; }
}
