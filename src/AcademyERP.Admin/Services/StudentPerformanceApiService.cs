using System.Net.Http.Json;
using AcademyERP.Admin.Models.StudentPerformance;

namespace AcademyERP.Admin.Services;

public class StudentPerformanceApiService
{
    private readonly HttpClient _http;


    public StudentPerformanceApiService(
        HttpClient http)
    {
        _http = http;
    }


    public async Task<StudentPerformanceResponse?> GetAsync(
        Guid studentId)
    {
        return await _http.GetFromJsonAsync<StudentPerformanceResponse>(
            $"api/students/{studentId}/performance");
    }
}