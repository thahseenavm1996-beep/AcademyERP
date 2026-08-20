using System.Net.Http.Json;
using AcademyERP.Application.DTOs.ScheduledClasses;

namespace AcademyERP.Admin.Services;

public class ScheduledClassApiService
{
    private readonly HttpClient _http;

    public ScheduledClassApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<ScheduledClassResponse>> GetAllAsync()
    {
        return await _http.GetFromJsonAsync<
            List<ScheduledClassResponse>>(
            "api/scheduledclasses")
            ?? new List<ScheduledClassResponse>();
    }
    public async Task<ScheduledClassResponse?> GetByIdAsync(Guid id)
{
    return await _http.GetFromJsonAsync<ScheduledClassResponse>(
        $"api/scheduledclasses/{id}");
}
public async Task<ScheduledClassResponse?> UpdateAsync(
    Guid id,
    UpdateScheduledClassRequest request)
{
    var response =
        await _http.PutAsJsonAsync(
            $"api/scheduledclasses/{id}",
            request);


    response.EnsureSuccessStatusCode();


    return await response.Content
        .ReadFromJsonAsync<ScheduledClassResponse>();
}
}