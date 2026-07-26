using System.Net.Http.Json;
using AcademyERP.Admin.Models.ClassReports;

namespace AcademyERP.Admin.Services;

public class ClassReportApiService
{
    private readonly HttpClient _httpClient;

    public ClassReportApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PagedResponse<ClassReportResponse>> GetAllAsync(
        ClassReportQueryRequest request)
    {
        var url =
            $"api/ClassReport?page={request.Page}" +
            $"&pageSize={request.PageSize}" +
            $"&studentId={request.StudentId}" +
            $"&teacherId={request.TeacherId}";

        return await _httpClient.GetFromJsonAsync<PagedResponse<ClassReportResponse>>(url)
               ?? new PagedResponse<ClassReportResponse>();
    }

    public async Task<ClassReportResponse?> GetByIdAsync(Guid id)
    {
        return await _httpClient.GetFromJsonAsync<ClassReportResponse>(
            $"api/ClassReport/{id}");
    }

    public async Task CreateAsync(CreateClassReportRequest request)
    {
        var response =
            await _httpClient.PostAsJsonAsync(
                "api/ClassReport",
                request);

        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateAsync(
        Guid id,
        UpdateClassReportRequest request)
    {
        var response =
            await _httpClient.PutAsJsonAsync(
                $"api/ClassReport/{id}",
                request);

        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(Guid id)
    {
        var response =
            await _httpClient.DeleteAsync(
                $"api/ClassReport/{id}");

        response.EnsureSuccessStatusCode();
    }
}