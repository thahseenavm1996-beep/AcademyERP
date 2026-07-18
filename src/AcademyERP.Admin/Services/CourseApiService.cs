using System.Net.Http.Json;
using AcademyERP.Admin.Models.Courses;

namespace AcademyERP.Admin.Services;

public class CourseApiService
{
    private readonly HttpClient _httpClient;

    public CourseApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<CourseResponse>> GetAllAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<CourseResponse>>(
            "api/courses") ?? new();
    }

    public async Task<CourseResponse?> GetByIdAsync(Guid id)
    {
        return await _httpClient.GetFromJsonAsync<CourseResponse>(
            $"api/courses/{id}");
    }

    public async Task CreateAsync(CreateCourseRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "api/courses",
            request);

        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateAsync(Guid id, UpdateCourseRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync(
            $"api/courses/{id}",
            request);

        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync(
            $"api/courses/{id}");

        response.EnsureSuccessStatusCode();
    }
}