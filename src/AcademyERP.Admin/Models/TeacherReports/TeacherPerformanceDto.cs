namespace AcademyERP.Admin.Models.TeacherReports;

public class TeacherPerformanceDto
{
    public Guid TeacherId { get; set; }

    public string TeacherName { get; set; } = string.Empty;

    public decimal Score { get; set; }

    public int ClassesCompleted { get; set; }

    public decimal AttendancePercentage { get; set; }
}