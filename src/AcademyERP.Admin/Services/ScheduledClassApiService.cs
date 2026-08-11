using System.Net.Http.Json;
using AcademyERP.Admin.Models;

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
}