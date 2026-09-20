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

  public async Task<StudentReportDashboardResponse?> GetDashboardAsync(
    string? courseName = null,
    int? gender = null)
{
    var url = "api/reports/students/dashboard";

    var parameters = new List<string>();

    if (!string.IsNullOrWhiteSpace(courseName))
        parameters.Add($"courseName={Uri.EscapeDataString(courseName)}");

    if (gender.HasValue)
        parameters.Add($"gender={gender.Value}");

    if (parameters.Count > 0)
        url += "?" + string.Join("&", parameters);

    return await _http.GetFromJsonAsync<StudentReportDashboardResponse>(url);
}
}