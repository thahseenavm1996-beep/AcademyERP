using System.Net.Http.Json;
using AcademyERP.Admin.Models.StudentReports;

namespace AcademyERP.Admin.Services;

public class StudentReportApiService
{
    private readonly HttpClient _http;

    public StudentReportApiService(
        HttpClient http)
    {
        _http = http;
    }


    public async Task<StudentReportResponse?> GetAsync(
        Guid studentId)
    {
        return await _http.GetFromJsonAsync<StudentReportResponse>(
            $"api/reports/students/{studentId}");
    }

    public async Task<StudentReportDashboardResponse?> GetDashboardAsync()
    {
        return await _http.GetFromJsonAsync<StudentReportDashboardResponse>(
            "api/reports/students/dashboard");
    }
}