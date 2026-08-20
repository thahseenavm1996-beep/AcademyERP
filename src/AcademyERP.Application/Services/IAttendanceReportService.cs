using AcademyERP.Application.DTOs.AttendanceReports;

namespace AcademyERP.Application.Services;

public interface IAttendanceReportService
{
    Task<AttendanceReportDashboardResponse> GetDashboardAsync();
}