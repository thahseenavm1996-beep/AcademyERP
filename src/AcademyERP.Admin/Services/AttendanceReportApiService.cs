using System.Net.Http.Json;
using AcademyERP.Admin.Models.AttendanceReports;

namespace AcademyERP.Admin.Services;

public class AttendanceReportApiService
{
    private readonly HttpClient _http;


    public AttendanceReportApiService(
        HttpClient http)
    {
        _http = http;
    }


    public async Task<AttendanceReportDashboardResponse?> GetAsync()
    {
        return await _http.GetFromJsonAsync<AttendanceReportDashboardResponse>(
            "api/reports/attendance");
    }
}