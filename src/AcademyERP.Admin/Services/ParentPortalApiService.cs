using System.Net.Http.Json;
using AcademyERP.Admin.Models.ParentPortal;

namespace AcademyERP.Admin.Services;

public class ParentPortalApiService
{
    private readonly HttpClient _http;

    public ParentPortalApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<ParentDashboardResponse?> GetDashboardAsync()
    {
        return await _http.GetFromJsonAsync<ParentDashboardResponse>(
            "api/parent-portal/dashboard");
    }

    public async Task<List<ParentChildSummaryDto>?> GetChildrenAsync()
    {
        return await _http.GetFromJsonAsync<List<ParentChildSummaryDto>>(
            "api/parent-portal/children");
    }

    public async Task<List<ParentScheduleDto>?> GetScheduleAsync(
        Guid? studentId = null,
        DateOnly? fromDate = null,
        DateOnly? toDate = null)
    {
        var query = new List<string>();

        if (studentId.HasValue)
            query.Add($"studentId={studentId.Value}");

        if (fromDate.HasValue)
            query.Add($"fromDate={fromDate.Value:yyyy-MM-dd}");

        if (toDate.HasValue)
            query.Add($"toDate={toDate.Value:yyyy-MM-dd}");

        var url = "api/parent-portal/schedule";

        if (query.Count > 0)
            url += "?" + string.Join("&", query);

        return await _http.GetFromJsonAsync<List<ParentScheduleDto>>(url);
    }

    public async Task<ParentAttendanceResponse?> GetAttendanceAsync(
        Guid? studentId = null,
        string? status = null,
        DateOnly? fromDate = null,
        DateOnly? toDate = null)
    {
        var query = new List<string>();

        if (studentId.HasValue)
            query.Add($"studentId={studentId.Value}");

        if (!string.IsNullOrWhiteSpace(status))
            query.Add($"status={Uri.EscapeDataString(status)}");

        if (fromDate.HasValue)
            query.Add($"fromDate={fromDate.Value:yyyy-MM-dd}");

        if (toDate.HasValue)
            query.Add($"toDate={toDate.Value:yyyy-MM-dd}");

        var url = "api/parent-portal/attendance";

        if (query.Count > 0)
            url += "?" + string.Join("&", query);

        return await _http.GetFromJsonAsync<ParentAttendanceResponse>(url);
    }

    public async Task<ParentFeesResponse?> GetFeesAsync(
        Guid? studentId = null)
    {
        var url = "api/parent-portal/fees";

        if (studentId.HasValue)
            url += $"?studentId={studentId.Value}";

        return await _http.GetFromJsonAsync<ParentFeesResponse>(url);
    }

    public async Task<List<ParentProgressDto>?> GetProgressAsync(
        Guid? studentId = null)
    {
        var url = "api/parent-portal/progress";

        if (studentId.HasValue)
            url += $"?studentId={studentId.Value}";

        return await _http.GetFromJsonAsync<List<ParentProgressDto>>(url);
    }

    public async Task<ParentProfileDto?> GetProfileAsync()
    {
        return await _http.GetFromJsonAsync<ParentProfileDto>(
            "api/parent-portal/profile");
    }

    public async Task<bool> UpdateProfileAsync(
        UpdateParentProfileRequest request)
    {
        var response = await _http.PutAsJsonAsync(
            "api/parent-portal/profile",
            request);

        return response.IsSuccessStatusCode;
    }
}