using System.Net.Http.Json;
using AcademyERP.Admin.Models;

namespace AcademyERP.Admin.Services;

public class TeachingScheduleApiService
{
    private readonly HttpClient _http;

    public TeachingScheduleApiService(HttpClient http)
    {
        _http = http;
    }

   /* public async Task<List<TeachingScheduleResponse>?> GetAllAsync()
    {
        return await _http.GetFromJsonAsync<List<TeachingScheduleResponse>>(
            "api/teachingschedules");
    }*/
    public async Task<List<TeachingScheduleResponse>?> GetAllAsync()
{
    var response = await _http.GetAsync("api/teachingschedules");

    Console.WriteLine($"Status Code: {response.StatusCode}");

    var content = await response.Content.ReadAsStringAsync();

    Console.WriteLine(content);

    if (!response.IsSuccessStatusCode)
        return null;

    return await response.Content.ReadFromJsonAsync<List<TeachingScheduleResponse>>();
}
public async Task<TeachingScheduleResponse?> CreateAsync(
    CreateTeachingScheduleRequest request)
{
    var response = await _http.PostAsJsonAsync(
        "api/teachingschedules",
        request);

    response.EnsureSuccessStatusCode();

    return await response.Content
        .ReadFromJsonAsync<TeachingScheduleResponse>();
}

}