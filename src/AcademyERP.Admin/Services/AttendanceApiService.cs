using AcademyERP.Admin.Models.Attendance;
using System.Net.Http.Json;

namespace AcademyERP.Admin.Services;

public class AttendanceApiService
{
    private readonly HttpClient _http;

    public AttendanceApiService(HttpClient http) => _http = http;

    public async Task<PagedResponse<AttendanceResponse>> GetHistoryAsync(AttendanceQueryRequest request) =>
        await _http.GetFromJsonAsync<PagedResponse<AttendanceResponse>>($"api/attendance?{ToQueryString(request)}") ?? new();

    public async Task<AttendanceSummaryResponse> GetSummaryAsync(AttendanceQueryRequest request) =>
        await _http.GetFromJsonAsync<AttendanceSummaryResponse>($"api/attendance/summary?{ToQueryString(request)}") ?? new();

    public async Task<List<AttendancePersonSummaryResponse>> GetStudentReportAsync(AttendanceQueryRequest request) =>
        await _http.GetFromJsonAsync<List<AttendancePersonSummaryResponse>>($"api/attendance/reports/students?{ToQueryString(request)}") ?? new();

    public async Task<List<AttendancePersonSummaryResponse>> GetTeacherReportAsync(AttendanceQueryRequest request) =>
        await _http.GetFromJsonAsync<List<AttendancePersonSummaryResponse>>($"api/attendance/reports/teachers?{ToQueryString(request)}") ?? new();

    public async Task CreateAsync(CreateAttendanceRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/attendance", request);
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateAsync(Guid id, UpdateAttendanceRequest request)
    {
        var response = await _http.PutAsJsonAsync($"api/attendance/{id}", request);
        response.EnsureSuccessStatusCode();
    }

    private static string ToQueryString(AttendanceQueryRequest request)
    {
        var values = new Dictionary<string, string?>
        {
            ["studentId"] = request.StudentId?.ToString(), ["teacherId"] = request.TeacherId?.ToString(),
            ["programId"] = request.ProgramId?.ToString(), ["status"] = request.Status?.ToString(),
            ["fromDate"] = request.FromDate?.ToString("yyyy-MM-dd"), ["toDate"] = request.ToDate?.ToString("yyyy-MM-dd"),
            ["page"] = request.Page.ToString(), ["pageSize"] = request.PageSize.ToString()
        };
        return string.Join("&", values.Where(x => !string.IsNullOrWhiteSpace(x.Value))
            .Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value!)}"));
    }
    public async Task<AttendanceResponse?> GetByEnrollmentDateAsync(
    Guid enrollmentId,
    DateTime date)
{
    return await _http.GetFromJsonAsync<AttendanceResponse>(
        $"api/attendance/by-enrollment-date?enrollmentId={enrollmentId}&date={date:yyyy-MM-dd}");
}
}
