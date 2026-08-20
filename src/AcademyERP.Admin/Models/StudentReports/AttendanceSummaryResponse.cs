namespace AcademyERP.Admin.Models.StudentReports;

public class AttendanceSummaryResponse
{
    public int TotalClasses { get; set; }

    public int Present { get; set; }

    public int Absent { get; set; }

    public int Late { get; set; }

    public int Leave { get; set; }

    public decimal AttendancePercentage { get; set; }
}