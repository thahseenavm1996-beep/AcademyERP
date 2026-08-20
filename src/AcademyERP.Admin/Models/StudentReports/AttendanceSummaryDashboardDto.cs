namespace AcademyERP.Admin.Models.StudentReports;

public class AttendanceSummaryDashboardDto
{
    public decimal PresentPercentage { get; set; }

    public decimal AbsentPercentage { get; set; }

    public decimal LeavePercentage { get; set; }

    public decimal LatePercentage { get; set; }
}