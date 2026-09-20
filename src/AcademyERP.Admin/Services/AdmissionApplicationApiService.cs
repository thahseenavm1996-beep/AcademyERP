using System.Net.Http.Json;
using AcademyERP.Admin.Models.Admissions;

namespace AcademyERP.Admin.Services;

public class AdmissionApplicationApiService
{
    private readonly HttpClient _httpClient;

    public AdmissionApplicationApiService(
        HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<AdmissionApplicationResponse>> GetAllAsync()
    {
        return await _httpClient
            .GetFromJsonAsync<List<AdmissionApplicationResponse>>(
                "api/AdmissionApplications")
            ?? new();
    }

    public async Task<AdmissionApplicationResponse?> GetByIdAsync(
        Guid id)
    {
        return await _httpClient
            .GetFromJsonAsync<AdmissionApplicationResponse>(
                $"api/AdmissionApplications/{id}");
    }
    public async Task<List<AvailableTeacherResponse>> GetAvailableTeachersAsync(Guid id)
    {
        return await _httpClient.GetFromJsonAsync<List<AvailableTeacherResponse>>(
            $"api/AdmissionApplications/{id}/available-teachers") ?? new();
    }
    public async Task AssignTeacherAsync(Guid id, Guid teacherId)
    {
        var response = await _httpClient.PutAsJsonAsync(
            $"api/AdmissionApplications/{id}/assigned-teacher", teacherId);
        response.EnsureSuccessStatusCode();
    }
    public async Task UpdateStatusAsync(
    Guid id,
    int status)
    {
        var response = await _httpClient.PutAsJsonAsync(
            $"api/AdmissionApplications/{id}/status",
            new
            {
                Status = status
            });

        response.EnsureSuccessStatusCode();
    }

    public async Task<bool> MarkAsViewedAsync(Guid id)
{
    var response = await _httpClient.PutAsync(
        $"api/AdmissionApplications/{id}/viewed",
        null);

    return response.IsSuccessStatusCode;
}
}
