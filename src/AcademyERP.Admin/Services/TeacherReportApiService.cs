using AcademyERP.Admin.Models.TeacherReports;
using System.Net.Http.Json;

namespace AcademyERP.Admin.Services;

public class TeacherReportApiService
{
    private readonly HttpClient _http;

    public TeacherReportApiService(HttpClient http)
    {
        _http = http;
    }


    public async Task<TeacherReportDashboardResponse?> GetDashboardAsync()
    {
        return await _http.GetFromJsonAsync<TeacherReportDashboardResponse>(
            "api/reports/teachers/dashboard");
    }
}