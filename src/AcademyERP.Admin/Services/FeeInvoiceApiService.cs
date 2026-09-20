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

    public async Task<List<FeeInvoiceResponse>> GetAllAsync()
{
    var response =
        await _httpClient.GetFromJsonAsync<
            ApiResponse<PagedResponse<FeeInvoiceResponse>>>(
            "api/FeeInvoices");

    return response?.Data?.Items
           ?? new List<FeeInvoiceResponse>();
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
