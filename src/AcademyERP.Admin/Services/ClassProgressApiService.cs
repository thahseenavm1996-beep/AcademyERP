using System.Net.Http.Json;
using AcademyERP.Admin.Models.ClassProgress;

namespace AcademyERP.Admin.Services;

public class ClassProgressApiService
{
    private readonly HttpClient _http;

    public ClassProgressApiService(HttpClient http)
    {
        _http = http;
    }


    public async Task CreateAsync(
        CreateClassProgressRequest request)
    {
        var response =
            await _http.PostAsJsonAsync(
                "api/classprogress",
                request);

        response.EnsureSuccessStatusCode();
    }


    public async Task<ClassProgressResponse?> GetByScheduledClassIdAsync(
        Guid scheduledClassId)
    {
        return await _http.GetFromJsonAsync<ClassProgressResponse>(
            $"api/classprogress/{scheduledClassId}");
    }
}