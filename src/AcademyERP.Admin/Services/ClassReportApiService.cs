using System.Net.Http.Json;
using AcademyERP.Admin.Models.ClassReports;
using System.Text.Json;
using AcademyERP.Domain.Enums;

namespace AcademyERP.Admin.Services;

public class ClassReportApiService
{
    private readonly HttpClient _httpClient;

    public ClassReportApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PagedResponse<ClassReportResponse>> GetAllAsync(
        ClassReportQueryRequest request)
    {
        var url =
            $"api/ClassReport?page={request.Page}" +
            $"&pageSize={request.PageSize}" +
            $"&studentId={request.StudentId}" +
            $"&teacherId={request.TeacherId}" +
            $"&studentName={Uri.EscapeDataString(request.StudentName ?? string.Empty)}" +
            $"&teacherName={Uri.EscapeDataString(request.TeacherName ?? string.Empty)}" +
            $"&fromDate={FormatDate(request.FromDate)}" +
            $"&toDate={FormatDate(request.ToDate)}" +
            $"&attendanceStatus={request.AttendanceStatus}" +
            $"&courseName={Uri.EscapeDataString(request.CourseName ?? string.Empty)}" +
$"&gender={request.Gender}" +
            $"&performanceRating={request.PerformanceRating}";
            

        return await _httpClient.GetFromJsonAsync<PagedResponse<ClassReportResponse>>(url)
               ?? new PagedResponse<ClassReportResponse>();
    }

    public async Task<byte[]> DownloadPdfAsync(
        ClassReportQueryRequest? request = null)
    {
        var url = "api/ClassReport/export/pdf";

        if (request is not null)
        {
            var parameters = new List<string>();

            if (request.StudentId.HasValue)
                parameters.Add($"studentId={request.StudentId.Value}");

            if (request.TeacherId.HasValue)
                parameters.Add($"teacherId={request.TeacherId.Value}");

            if (!string.IsNullOrWhiteSpace(request.StudentName))
            {
                parameters.Add(
                    $"studentName={Uri.EscapeDataString(request.StudentName)}");
            }

            if (!string.IsNullOrWhiteSpace(request.TeacherName))
            {
                parameters.Add(
                    $"teacherName={Uri.EscapeDataString(request.TeacherName)}");
            }
if (!string.IsNullOrWhiteSpace(request.CourseName))
{
    parameters.Add(
        $"courseName={Uri.EscapeDataString(request.CourseName)}");
}

if (request.Gender.HasValue)
{
    parameters.Add(
        $"gender={request.Gender.Value}");
}
            if (request.FromDate.HasValue)
                parameters.Add($"fromDate={FormatDate(request.FromDate)}");

            if (request.ToDate.HasValue)
                parameters.Add($"toDate={FormatDate(request.ToDate)}");

            if (request.AttendanceStatus.HasValue)
            {
                parameters.Add(
                    $"attendanceStatus={request.AttendanceStatus.Value}");
            }

            if (request.PerformanceRating.HasValue)
            {
                parameters.Add(
                    $"performanceRating={request.PerformanceRating.Value}");
            }

            if (parameters.Count > 0)
                url += "?" + string.Join("&", parameters);
        }

        return await _httpClient.GetByteArrayAsync(url);
    }

    private static string FormatDate(DateTime? date)
    {
        return date.HasValue
            ? date.Value.ToString("yyyy-MM-dd")
            : string.Empty;
    }

    public async Task<ClassReportResponse?> GetByIdAsync(Guid id)
    {
        return await _httpClient.GetFromJsonAsync<ClassReportResponse>(
            $"api/ClassReport/{id}");
    }

    public async Task CreateAsync(CreateClassReportRequest request)
    {
        var response =
            await _httpClient.PostAsJsonAsync(
                "api/ClassReport",
                request);

        if (!response.IsSuccessStatusCode)
        {
            var error =
                await response.Content.ReadAsStringAsync();

            try
            {
                var json =
                    JsonSerializer.Deserialize<JsonElement>(error);

                if (json.TryGetProperty("message", out var message))
                {
                    throw new Exception(
                        message.GetString()
                        ?? "Unable to create class report.");
                }
            }
            catch (JsonException)
            {
                throw new Exception(error);
            }
        }
    }

    public async Task UpdateAsync(
        Guid id,
        UpdateClassReportRequest request)
    {
        var response =
            await _httpClient.PutAsJsonAsync(
                $"api/ClassReport/{id}",
                request);

        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(Guid id)
    {
        var response =
            await _httpClient.DeleteAsync(
                $"api/ClassReport/{id}");

        response.EnsureSuccessStatusCode();
    }

    public async Task<ClassReportResponse?> GetByScheduledClassIdAsync(
        Guid scheduledClassId)
    {
        return await _httpClient.GetFromJsonAsync<ClassReportResponse>(
            $"api/ClassReport/scheduled-class/{scheduledClassId}");
    }

    public async Task<List<TeacherReportMonitoringResponse>> GetTeacherReportMonitoringAsync(
    DateTime? fromDate = null,
    DateTime? toDate = null,
    Guid? teacherId = null,
    string? courseName = null,
    Gender? gender = null,
    bool? reportSubmitted = null)
{
    var query = new List<string>();

    if (fromDate.HasValue)
        query.Add($"fromDate={Uri.EscapeDataString(fromDate.Value.ToString("yyyy-MM-dd"))}");

    if (toDate.HasValue)
        query.Add($"toDate={Uri.EscapeDataString(toDate.Value.ToString("yyyy-MM-dd"))}");

    if (teacherId.HasValue)
        query.Add($"teacherId={teacherId.Value}");

    if (!string.IsNullOrWhiteSpace(courseName))
        query.Add($"courseName={Uri.EscapeDataString(courseName)}");

    if (gender.HasValue)
        query.Add($"gender={gender.Value}");

    if (reportSubmitted.HasValue)
        query.Add(
            $"reportSubmitted={reportSubmitted.Value.ToString().ToLowerInvariant()}");

    var url = "api/ClassReport/teacher-monitoring";

    if (query.Count > 0)
        url += "?" + string.Join("&", query);

    return await _httpClient
        .GetFromJsonAsync<List<TeacherReportMonitoringResponse>>(url)
        ?? new List<TeacherReportMonitoringResponse>();
}

public async Task<byte[]> DownloadTeacherReportMonitoringPdfAsync(
    DateTime? fromDate = null,
    DateTime? toDate = null,
    Guid? teacherId = null,
    string? courseName = null,
    Gender? gender = null,
    bool? reportSubmitted = null)
{
    var parameters = new List<string>();

    if (fromDate.HasValue)
        parameters.Add($"fromDate={FormatDate(fromDate)}");

    if (toDate.HasValue)
        parameters.Add($"toDate={FormatDate(toDate)}");

    if (teacherId.HasValue)
        parameters.Add($"teacherId={teacherId.Value}");

    if (!string.IsNullOrWhiteSpace(courseName))
        parameters.Add(
            $"courseName={Uri.EscapeDataString(courseName)}");

    if (gender.HasValue)
        parameters.Add($"gender={gender.Value}");

    if (reportSubmitted.HasValue)
        parameters.Add(
            $"reportSubmitted={reportSubmitted.Value.ToString().ToLowerInvariant()}");

    var url = "api/ClassReport/teacher-monitoring/export/pdf";

    if (parameters.Count > 0)
        url += "?" + string.Join("&", parameters);

    return await _httpClient.GetByteArrayAsync(url);

}

public async Task<byte[]> DownloadImageAsync(
    ClassReportQueryRequest? request = null)
{
    var url = "api/ClassReport/export/image";

    if (request is not null)
    {
        var parameters = new List<string>();

        if (request.StudentId.HasValue)
            parameters.Add($"studentId={request.StudentId.Value}");

        if (request.TeacherId.HasValue)
            parameters.Add($"teacherId={request.TeacherId.Value}");

        if (!string.IsNullOrWhiteSpace(request.StudentName))
        {
            parameters.Add(
                $"studentName={Uri.EscapeDataString(request.StudentName)}");
        }

        if (!string.IsNullOrWhiteSpace(request.TeacherName))
        {
            parameters.Add(
                $"teacherName={Uri.EscapeDataString(request.TeacherName)}");
        }

        if (request.FromDate.HasValue)
        {
            parameters.Add(
                $"fromDate={FormatDate(request.FromDate)}");
        }

        if (request.ToDate.HasValue)
        {
            parameters.Add(
                $"toDate={FormatDate(request.ToDate)}");
        }

        if (request.CourseName != null)
        {
            parameters.Add(
                $"courseName={Uri.EscapeDataString(request.CourseName)}");
        }

        if (request.Gender.HasValue)
            parameters.Add($"gender={request.Gender.Value}");

        if (request.AttendanceStatus.HasValue)
            parameters.Add(
                $"attendanceStatus={request.AttendanceStatus.Value}");

        if (request.PerformanceRating.HasValue)
            parameters.Add(
                $"performanceRating={request.PerformanceRating.Value}");

        if (parameters.Count > 0)
            url += "?" + string.Join("&", parameters);
    }

    var response = await _httpClient.GetAsync(url);

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
