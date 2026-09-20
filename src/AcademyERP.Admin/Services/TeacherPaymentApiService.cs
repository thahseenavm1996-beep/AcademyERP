using System.Net.Http.Json;
using AcademyERP.Admin.Models;
using AcademyERP.Admin.Models.ClassReports;
using AcademyERP.Admin.Models.TeacherPayments;

namespace AcademyERP.Admin.Services;

public class TeacherPaymentApiService
{
    private readonly HttpClient _httpClient;

    public TeacherPaymentApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<TeacherPaymentConfigurationResponse>>
        GetConfigurationsAsync(
            Guid? teacherId = null,
            Guid? enrollmentId = null)
    {
        var parameters = new List<string>();

        if (teacherId.HasValue)
            parameters.Add($"teacherId={teacherId.Value}");

        if (enrollmentId.HasValue)
            parameters.Add($"enrollmentId={enrollmentId.Value}");

        var url = "api/TeacherPayments/configurations";

        if (parameters.Count > 0)
            url += "?" + string.Join("&", parameters);

        var result = await _httpClient
            .GetFromJsonAsync<ApiResponse<List<TeacherPaymentConfigurationResponse>>>(
                url);

        return result?.Data ?? [];
    }

    public async Task<TeacherPaymentConfigurationResponse?>
        GetConfigurationAsync(Guid id)
    {
        var result = await _httpClient
            .GetFromJsonAsync<ApiResponse<TeacherPaymentConfigurationResponse>>(
                $"api/TeacherPayments/configurations/{id}");

        return result?.Data;
    }

    public async Task<TeacherPaymentConfigurationResponse?>
        CreateConfigurationAsync(
            CreateTeacherPaymentConfigurationRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "api/TeacherPayments/configurations",
            request);

        if (!response.IsSuccessStatusCode)
            return null;

        var result = await response.Content
            .ReadFromJsonAsync<ApiResponse<TeacherPaymentConfigurationResponse>>();

        return result?.Data;
    }

    public async Task<TeacherPaymentConfigurationResponse?>
        UpdateConfigurationAsync(
            Guid id,
            UpdateTeacherPaymentConfigurationRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync(
            $"api/TeacherPayments/configurations/{id}",
            request);

        if (!response.IsSuccessStatusCode)
            return null;

        var result = await response.Content
            .ReadFromJsonAsync<ApiResponse<TeacherPaymentConfigurationResponse>>();

        return result?.Data;
    }

    public async Task<bool> DeleteConfigurationAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync(
            $"api/TeacherPayments/configurations/{id}");

        return response.IsSuccessStatusCode;
    }

    public async Task<PagedResponse<TeacherPaymentResponse>>
        GetPaymentsAsync(
            TeacherPaymentQueryRequest request)
    {
        var parameters = new List<string>
        {
            $"page={request.Page}",
            $"pageSize={request.PageSize}"
        };

        if (request.TeacherId.HasValue)
            parameters.Add($"teacherId={request.TeacherId.Value}");

        if (request.EnrollmentId.HasValue)
            parameters.Add($"enrollmentId={request.EnrollmentId.Value}");

        if (!string.IsNullOrWhiteSpace(request.TeacherName))
            parameters.Add(
                $"teacherName={Uri.EscapeDataString(request.TeacherName)}");

        if (!string.IsNullOrWhiteSpace(request.StudentName))
            parameters.Add(
                $"studentName={Uri.EscapeDataString(request.StudentName)}");

        if (request.FromDate.HasValue)
            parameters.Add(
                $"fromDate={request.FromDate.Value:yyyy-MM-dd}");

        if (request.ToDate.HasValue)
            parameters.Add(
                $"toDate={request.ToDate.Value:yyyy-MM-dd}");

        var url =
            "api/TeacherPayments?" +
            string.Join("&", parameters);

        var result = await _httpClient
            .GetFromJsonAsync<ApiResponse<PagedResponse<TeacherPaymentResponse>>>(
                url);

        return result?.Data ?? new PagedResponse<TeacherPaymentResponse>();
    }

    public async Task<TeacherPaymentResponse?>
        GetPaymentAsync(Guid id)
    {
        var result = await _httpClient
            .GetFromJsonAsync<ApiResponse<TeacherPaymentResponse>>(
                $"api/TeacherPayments/{id}");

        return result?.Data;
    }

    public async Task<TeacherPaymentResponse?>
        MarkPaidAsync(
            Guid id,
            MarkTeacherPaymentPaidRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(
            $"api/TeacherPayments/{id}/mark-paid",
            request);

        if (!response.IsSuccessStatusCode)
            return null;

        var result = await response.Content
            .ReadFromJsonAsync<ApiResponse<TeacherPaymentResponse>>();

        return result?.Data;
    }

    public async Task<TeacherPaymentResponse?>
        CancelAsync(Guid id)
    {
        var response = await _httpClient.PostAsync(
            $"api/TeacherPayments/{id}/cancel",
            null);

        if (!response.IsSuccessStatusCode)
            return null;

        var result = await response.Content
            .ReadFromJsonAsync<ApiResponse<TeacherPaymentResponse>>();

        return result?.Data;
    }

    public async Task<int> GenerateAsync(DateTime? asOfDate = null)
    {
        var url = "api/TeacherPayments/generate";

        if (asOfDate.HasValue)
            url += $"?asOfDate={asOfDate.Value:yyyy-MM-dd}";

        var response = await _httpClient.PostAsync(
            url,
            null);

        if (!response.IsSuccessStatusCode)
            return 0;

        var result = await response.Content
            .ReadFromJsonAsync<ApiResponse<int>>();

        return result?.Data ?? 0;

    }

    public async Task<TeacherPaymentConfigurationResponse?>
    StopConfigurationAsync(Guid id, DateTime stopDate)
{
    var response = await _httpClient.PostAsync(
        $"api/TeacherPayments/configurations/{id}/stop?stopDate={stopDate:yyyy-MM-dd}",
        null);

    if (!response.IsSuccessStatusCode)
        return null;

    var result = await response.Content
        .ReadFromJsonAsync<ApiResponse<TeacherPaymentConfigurationResponse>>();

    return result?.Data;
}

public async Task<TeacherPaymentConfigurationResponse?>
    PauseConfigurationAsync(
        Guid id,
        DateTime pauseStartDate,
        string? reason)
{
    var url =
        $"api/TeacherPayments/configurations/{id}/pause" +
        $"?pauseStartDate={pauseStartDate:yyyy-MM-dd}";

    if (!string.IsNullOrWhiteSpace(reason))
    {
        url += $"&reason={Uri.EscapeDataString(reason)}";
    }

    var response = await _httpClient.PostAsync(url, null);

    if (!response.IsSuccessStatusCode)
        return null;

    var result =
        await response.Content
            .ReadFromJsonAsync<
                ApiResponse<TeacherPaymentConfigurationResponse>>();

    return result?.Data;
}


public async Task<TeacherPaymentConfigurationResponse?>
    ResumeConfigurationAsync(
        Guid id,
        DateTime resumeDate)
{
    var response = await _httpClient.PostAsync(
        $"api/TeacherPayments/configurations/{id}/resume" +
        $"?resumeDate={resumeDate:yyyy-MM-dd}",
        null);

    if (!response.IsSuccessStatusCode)
        return null;

    var result =
        await response.Content
            .ReadFromJsonAsync<
                ApiResponse<TeacherPaymentConfigurationResponse>>();

    return result?.Data;
}
}
