using AcademyERP.Admin.Models;
using System.Net.Http.Json;
using AcademyERP.Admin.Models.ClassReports;
using AcademyERP.Admin.Models.Enrollments;

namespace AcademyERP.Admin.Services;

public class FeeInvoiceApiService
{
    private readonly HttpClient _httpClient;

    public FeeInvoiceApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PagedResponse<FeeInvoiceResponse>> GetAllAsync(
    FeeInvoiceQueryRequest request)
{
    var query = new List<string>
    {
        $"page={request.Page}",
        $"pageSize={request.PageSize}"
    };

    if (!string.IsNullOrWhiteSpace(request.StudentName))
        query.Add($"studentName={Uri.EscapeDataString(request.StudentName)}");

    if (!string.IsNullOrWhiteSpace(request.TeacherName))
        query.Add($"teacherName={Uri.EscapeDataString(request.TeacherName)}");

    if (request.TeacherId.HasValue)
        query.Add($"teacherId={request.TeacherId.Value}");

    if (request.CourseId.HasValue)
        query.Add($"courseId={request.CourseId.Value}");

    if (!string.IsNullOrWhiteSpace(request.Gender))
        query.Add($"gender={Uri.EscapeDataString(request.Gender)}");

    if (request.Status.HasValue)
        query.Add($"status={request.Status.Value}");

    if (request.StudentId.HasValue)
        query.Add($"studentId={request.StudentId.Value}");

    var url = $"api/FeeInvoices?{string.Join("&", query)}";

    var response =
        await _httpClient.GetFromJsonAsync<
            ApiResponse<PagedResponse<FeeInvoiceResponse>>>(url);

    return response?.Data
        ?? new PagedResponse<FeeInvoiceResponse>
        {
            Items = new List<FeeInvoiceResponse>(),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = 0,
            TotalPages = 0
        };
}

public async Task<List<FeeInvoiceResponse>> GetAllForExportAsync(
    FeeInvoiceQueryRequest request)
{
    var query = new List<string>();

    if (!string.IsNullOrWhiteSpace(request.StudentName))
        query.Add(
            $"studentName={Uri.EscapeDataString(request.StudentName)}");

    if (!string.IsNullOrWhiteSpace(request.TeacherName))
        query.Add(
            $"teacherName={Uri.EscapeDataString(request.TeacherName)}");

    if (request.TeacherId.HasValue)
        query.Add($"teacherId={request.TeacherId.Value}");

    if (request.CourseId.HasValue)
        query.Add($"courseId={request.CourseId.Value}");

    if (!string.IsNullOrWhiteSpace(request.Gender))
        query.Add(
            $"gender={Uri.EscapeDataString(request.Gender)}");

    if (request.Status.HasValue)
        query.Add($"status={request.Status.Value}");

    if (request.StudentId.HasValue)
        query.Add($"studentId={request.StudentId.Value}");

    var url = "api/FeeInvoices/export/data";

    if (query.Count > 0)
        url += "?" + string.Join("&", query);

    var response =
        await _httpClient.GetFromJsonAsync<
            ApiResponse<List<FeeInvoiceResponse>>>(url);

    return response?.Data
        ?? new List<FeeInvoiceResponse>();
}

public async Task<byte[]> DownloadFilteredPdfAsync(
    FeeInvoiceQueryRequest request)
{
    var query = new List<string>();

    if (!string.IsNullOrWhiteSpace(request.StudentName))
        query.Add(
            $"studentName={Uri.EscapeDataString(request.StudentName)}");

    if (!string.IsNullOrWhiteSpace(request.TeacherName))
        query.Add(
            $"teacherName={Uri.EscapeDataString(request.TeacherName)}");

    if (request.TeacherId.HasValue)
        query.Add($"teacherId={request.TeacherId.Value}");

    if (request.CourseId.HasValue)
        query.Add($"courseId={request.CourseId.Value}");

    if (!string.IsNullOrWhiteSpace(request.Gender))
        query.Add(
            $"gender={Uri.EscapeDataString(request.Gender)}");

    if (request.Status.HasValue)
        query.Add($"status={request.Status.Value}");

    if (request.StudentId.HasValue)
        query.Add($"studentId={request.StudentId.Value}");

    var url = "api/FeeInvoices/export/pdf";

    if (query.Count > 0)
        url += "?" + string.Join("&", query);

    return await _httpClient.GetByteArrayAsync(url);
}
    public async Task CreateAsync(
        CreateFeeInvoiceRequest request)
    {
        await _httpClient.PostAsJsonAsync(
            "api/FeeInvoices",
            request);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _httpClient.DeleteAsync(
            $"api/FeeInvoices/{id}");
    }
    public async Task<FeeInvoiceResponse?> GetByIdAsync(Guid id)
{
    var response =
        await _httpClient.GetFromJsonAsync<
            ApiResponse<FeeInvoiceResponse>>(
            $"api/FeeInvoices/{id}");

    return response?.Data;
}

public async Task UpdateAsync(
    Guid id,
    UpdateFeeInvoiceRequest request)
{
    await _httpClient.PutAsJsonAsync(
        $"api/FeeInvoices/{id}",
        request);
}

public async Task<byte[]> DownloadPdfAsync(Guid id)
{
    return await _httpClient.GetByteArrayAsync(
        $"api/FeeInvoices/{id}/export/pdf");
}

public async Task<int> GenerateAsync(DateTime? asOfDate = null)
{
    var url = "api/FeeInvoices/generate";
    if (asOfDate.HasValue)
        url += $"?asOfDate={asOfDate.Value:yyyy-MM-dd}";

    var response = await _httpClient.PostAsync(url, null);
    response.EnsureSuccessStatusCode();

    var result = await response.Content
        .ReadFromJsonAsync<ApiResponse<int>>();
    return result?.Data ?? 0;
}

public async Task<EnrollmentResponse?> PauseEnrollmentAsync(
    Guid enrollmentId,
    DateTime pauseStartDate,
    string? reason)
{
    var url = $"api/FeeInvoices/enrollments/{enrollmentId}/pause" +
        $"?pauseStartDate={pauseStartDate:yyyy-MM-dd}";
    if (!string.IsNullOrWhiteSpace(reason))
        url += $"&reason={Uri.EscapeDataString(reason)}";

    var response = await _httpClient.PostAsync(url, null);
    response.EnsureSuccessStatusCode();

    var result = await response.Content
        .ReadFromJsonAsync<ApiResponse<EnrollmentResponse>>();
    return result?.Data;
}

public async Task<EnrollmentResponse?> ResumeEnrollmentAsync(
    Guid enrollmentId,
    DateTime resumeDate)
{
    var response = await _httpClient.PostAsync(
        $"api/FeeInvoices/enrollments/{enrollmentId}/resume" +
        $"?resumeDate={resumeDate:yyyy-MM-dd}",
        null);
    response.EnsureSuccessStatusCode();

    var result = await response.Content
        .ReadFromJsonAsync<ApiResponse<EnrollmentResponse>>();
    return result?.Data;
}

public async Task<List<FeeInvoiceResponse>>
    GetTeacherOutstandingAsync()
{
    return await _httpClient.GetFromJsonAsync<
        List<FeeInvoiceResponse>>(
            "api/FeeInvoices/teacher-outstanding")
        ?? new List<FeeInvoiceResponse>();
}
}
