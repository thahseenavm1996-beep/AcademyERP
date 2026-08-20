namespace AcademyERP.Admin.Models.AttendanceReports;

public class AttendanceReportDashboardResponse
{
    public int TotalClasses { get; set; }

    public int CompletedClasses { get; set; }

    public int TotalAttendanceRecords { get; set; }

    public decimal AverageAttendance { get; set; }


    public List<AttendanceTrendDto> AttendanceTrend { get; set; }
        = new();


    public List<AttendanceStatusDistributionDto> StatusDistribution { get; set; }
        = new();


    public List<CourseAttendanceDto> CourseAttendance { get; set; }
        = new();


    public List<StudentAttendanceAlertDto> StudentsNeedingAttention { get; set; }
        = new();


    public List<TeacherAttendanceSummaryDto> TeacherSummary { get; set; }
        = new();
}