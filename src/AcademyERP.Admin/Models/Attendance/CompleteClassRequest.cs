using AcademyERP.Admin.Models.ClassProgress;
using AcademyERP.Admin.Models.ClassReports;

namespace AcademyERP.Admin.Models.Attendance;

public class CompleteClassRequest
{
    public CreateAttendanceRequest Attendance { get; set; } = new();

    public CreateClassProgressRequest Progress { get; set; } = new();

    public CreateClassReportRequest Report { get; set; } = new();
}