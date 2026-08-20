using System.Net.Http.Json;
using AcademyERP.Admin.Models.TeacherDashboard;
using AcademyERP.Application.DTOs.ScheduledClasses;

namespace AcademyERP.Admin.Services;

public class TeacherDashboardApiService
{
    private readonly HttpClient _httpClient;


    public TeacherDashboardApiService(
        HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

   public async Task<TeacherDashboardResponse?> GetDashboardAsync()
{
    return await _httpClient
        .GetFromJsonAsync<TeacherDashboardResponse>(
            "api/teacher/dashboard");
}
public async Task StartClassAsync(Guid scheduledClassId)
{
    var response = await _httpClient.PostAsync(
        $"api/scheduledclasses/{scheduledClassId}/start",
        null);

    response.EnsureSuccessStatusCode();
}
public async Task CompleteClassAsync(
    Guid scheduledClassId,
    CompleteScheduledClassRequest request)
{
    var response =
        await _httpClient.PostAsJsonAsync(
            $"api/scheduledclasses/{scheduledClassId}/complete",
            request);

    response.EnsureSuccessStatusCode();
}

}