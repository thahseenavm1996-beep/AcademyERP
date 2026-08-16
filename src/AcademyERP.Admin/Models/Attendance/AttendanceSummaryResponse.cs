namespace AcademyERP.Admin.Models.Attendance;

public class AttendanceSummaryResponse
{
    public int Total { get; set; }
    public int Present { get; set; }
    public int Absent { get; set; }
    public int Late { get; set; }
    public int Leave { get; set; }
    public decimal AttendanceRate { get; set; }
}
