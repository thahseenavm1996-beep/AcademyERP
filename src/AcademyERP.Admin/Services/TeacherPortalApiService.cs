using System.Net.Http.Json;
using AcademyERP.Admin.Models.TeacherPortal;
using CreateTeacherBulkReportsRequest = AcademyERP.Application.DTOs.TeacherPortal.CreateTeacherBulkReportsRequest;
using CreateTeacherBulkReportsResponse = AcademyERP.Application.DTOs.TeacherPortal.CreateTeacherBulkReportsResponse;
using AcademyERP.Admin.Models.ReportUnlockRequests;
using CreateReportUnlockRequest =
    AcademyERP.Application.DTOs.ReportUnlockRequests.CreateReportUnlockRequest;


namespace AcademyERP.Admin.Services;


public class TeacherPortalApiService
{

private readonly HttpClient _http;


public TeacherPortalApiService(HttpClient http)
{
    _http=http;
}



public async Task<List<TeacherClassDto>> GetClassesAsync()
{
return await _http.GetFromJsonAsync<List<TeacherClassDto>>
("api/teacher-portal/classes")
?? [];
}



public async Task<List<TeacherStudentDto>> GetStudentsAsync()
{
return await _http.GetFromJsonAsync<List<TeacherStudentDto>>
("api/teacher-portal/students")
?? [];
}



public async Task<List<TeacherClassDto>> GetScheduleAsync()
{
return await _http.GetFromJsonAsync<List<TeacherClassDto>>
("api/teacher-portal/schedule")
?? [];
}



public async Task<List<TeacherAttendanceDto>> GetAttendanceAsync()
{
    return await _http.GetFromJsonAsync<List<TeacherAttendanceDto>>(
        "api/teacher-portal/attendance")
        ?? [];
}

public async Task<TeacherAttendanceDto?> MarkAttendanceAsync(
    Guid scheduledClassId,
    AcademyERP.Domain.Enums.AttendanceStatus status,
    string? remarks = null)
{
    var request =
        new AcademyERP.Application.DTOs.Attendance.CreateAttendanceRequest
        {
            ScheduledClassId = scheduledClassId,
            Status = status,
            Remarks = remarks
        };

    var response = await _http.PostAsJsonAsync(
        "api/teacher-portal/attendance",
        request);

    if (!response.IsSuccessStatusCode)
        return null;

    return await response.Content
        .ReadFromJsonAsync<TeacherAttendanceDto>();
}

public async Task<List<TeacherReportDto>> GetReportsAsync()
{
    return await _http.GetFromJsonAsync<List<TeacherReportDto>>(
        "api/teacher-portal/reports")
        ?? [];
}


public async Task<byte[]> DownloadReportsPdfAsync(
    DateTime? fromDate = null,
    DateTime? toDate = null,
    string? courseName = null,
    AcademyERP.Domain.Enums.Gender? gender = null)
{
    var query = new List<string>();

    if (fromDate.HasValue)
        query.Add($"fromDate={Uri.EscapeDataString(fromDate.Value.ToString("yyyy-MM-dd"))}");

    if (toDate.HasValue)
        query.Add($"toDate={Uri.EscapeDataString(toDate.Value.ToString("yyyy-MM-dd"))}");

    if (!string.IsNullOrWhiteSpace(courseName))
        query.Add($"courseName={Uri.EscapeDataString(courseName)}");

    if (gender.HasValue)
        query.Add($"gender={Uri.EscapeDataString(gender.Value.ToString())}");

    var url = "api/teacher-portal/reports/pdf";

    if (query.Count > 0)
        url += "?" + string.Join("&", query);

    var response = await _http.GetAsync(url);

    if (!response.IsSuccessStatusCode)
    {
        var message = await response.Content.ReadAsStringAsync();

        throw new HttpRequestException(
            string.IsNullOrWhiteSpace(message)
                ? $"Unable to download class reports. HTTP {(int)response.StatusCode}."
                : message);
    }

    return await response.Content.ReadAsByteArrayAsync();
}


public async Task<CreateTeacherBulkReportsResponse> CreateBulkReportsAsync(
    CreateTeacherBulkReportsRequest request)
{
    var response = await _http.PostAsJsonAsync(
        "api/teacher-portal/reports/bulk",
        request);

    if (!response.IsSuccessStatusCode)
    {
        var message =
            await response.Content.ReadAsStringAsync();

        throw new HttpRequestException(
            string.IsNullOrWhiteSpace(message)
                ? $"Unable to create class reports. HTTP {(int)response.StatusCode}."
                : message);
    }

    return await response.Content
        .ReadFromJsonAsync<CreateTeacherBulkReportsResponse>()
        ?? new CreateTeacherBulkReportsResponse();
}


public async Task<TeacherProfileDto?> GetProfileAsync()
{
    return await _http.GetFromJsonAsync<TeacherProfileDto>(
        "api/teacher-portal/profile");
}


public async Task<List<ReportUnlockRequestResponse>>
    GetMyReportUnlockRequestsAsync()
{
    return await _http.GetFromJsonAsync<
        List<ReportUnlockRequestResponse>>(
        "api/report-unlock-requests/teacher")
        ?? new List<ReportUnlockRequestResponse>();
}

public async Task<bool> RequestReportUnlockAsync(
    Guid scheduledClassId,
    string reason)
{
    var response = await _http.PostAsJsonAsync(
        "api/report-unlock-requests",
        new CreateReportUnlockRequest
        {
            ScheduledClassId = scheduledClassId,
            Reason = reason
        });

    return response.IsSuccessStatusCode;
}

public async Task<byte[]> DownloadReportsImageAsync(
    DateTime? fromDate = null,
    DateTime? toDate = null,
    string? courseName = null,
    AcademyERP.Domain.Enums.Gender? gender = null)
{
    var query = new List<string>();

    if (fromDate.HasValue)
        query.Add(
            $"fromDate={Uri.EscapeDataString(fromDate.Value.ToString("yyyy-MM-dd"))}");

    if (toDate.HasValue)
        query.Add(
            $"toDate={Uri.EscapeDataString(toDate.Value.ToString("yyyy-MM-dd"))}");

    if (!string.IsNullOrWhiteSpace(courseName))
        query.Add(
            $"courseName={Uri.EscapeDataString(courseName)}");

    if (gender.HasValue)
        query.Add(
            $"gender={Uri.EscapeDataString(gender.Value.ToString())}");

    var url = "api/teacher-portal/reports/image";

    if (query.Count > 0)
        url += "?" + string.Join("&", query);

    var response = await _http.GetAsync(url);

    if (!response.IsSuccessStatusCode)
    {
        var message =
            await response.Content.ReadAsStringAsync();

        throw new HttpRequestException(
            string.IsNullOrWhiteSpace(message)
                ? $"Unable to download class report image. HTTP {(int)response.StatusCode}."
                : message);
    }

    return await response.Content.ReadAsByteArrayAsync();
}

}
