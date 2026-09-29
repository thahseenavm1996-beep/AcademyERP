using System.Net.Http.Json;
using AcademyERP.Admin.Models.TeacherRegistrations;

namespace AcademyERP.Admin.Services;

public class TeacherRegistrationApiService
{
    private readonly HttpClient _http;

    public TeacherRegistrationApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<TeacherRegistrationResponse>> GetAllAsync()
    {
        return await _http.GetFromJsonAsync<List<TeacherRegistrationResponse>>(
            "api/teacher-registrations")
            ?? new List<TeacherRegistrationResponse>();
    }

    public async Task<List<TeacherRegistrationResponse>> GetPendingAsync()
    {
        return await _http.GetFromJsonAsync<List<TeacherRegistrationResponse>>(
            "api/teacher-registrations/pending")
            ?? new List<TeacherRegistrationResponse>();
    }

    public async Task<TeacherRegistrationResponse?> GetByIdAsync(Guid id)
    {
        return await _http.GetFromJsonAsync<TeacherRegistrationResponse>(
            $"api/teacher-registrations/{id}");
    }

    public async Task<bool> ApproveAsync(
    Guid id,
    ApproveTeacherRegistrationRequest request)
{
    var response = await _http.PostAsJsonAsync(
        $"api/teacher-registrations/{id}/approve",
        request);

    if (!response.IsSuccessStatusCode)
    {
        var error = await response.Content.ReadAsStringAsync();

        throw new InvalidOperationException(
            string.IsNullOrWhiteSpace(error)
                ? $"Approval failed with status code {(int)response.StatusCode}."
                : error);
    }

    return true;
}

    public async Task<bool> RejectAsync(
    Guid id,
    string? reviewRemarks)
{
    var response = await _http.PostAsJsonAsync(
        $"api/teacher-registrations/{id}/reject",
        reviewRemarks);

    return response.IsSuccessStatusCode;
}

public async Task<byte[]> DownloadPdfAsync(
    string? search,
    int gender,
    int status)
{
    var query = new List<string>();

    if (!string.IsNullOrWhiteSpace(search))
    {
        query.Add(
            $"search={Uri.EscapeDataString(search)}");
    }

    if (gender != 0)
        query.Add($"gender={gender}");

    if (status != 0)
        query.Add($"status={status}");

    var url = "api/teacher-registrations/export/pdf";

    if (query.Count > 0)
        url += "?" + string.Join("&", query);

    return await _http.GetByteArrayAsync(url);
}
}