using System.Net.Http.Json;
using AcademyERP.Admin.Models.Enrollments;

namespace AcademyERP.Admin.Services;

public class EnrollmentApiService
{
    private readonly HttpClient _httpClient;

    public EnrollmentApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<EnrollmentResponse>> GetAllAsync()
    {
        return await _httpClient
            .GetFromJsonAsync<List<EnrollmentResponse>>(
                "api/enrollments")
            ?? new List<EnrollmentResponse>();
    }

    public async Task<EnrollmentResponse?> GetByIdAsync(Guid id)
    {
        var response = await _httpClient.GetAsync(
            $"api/enrollments/{id}");

        if (response.StatusCode ==
            System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content
            .ReadFromJsonAsync<EnrollmentResponse>();
    }

    public async Task<EnrollmentResponse?> CreateAsync(
        CreateEnrollmentRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "api/enrollments",
            request);

        response.EnsureSuccessStatusCode();

        return await response.Content
            .ReadFromJsonAsync<EnrollmentResponse>();
    }

    public async Task<EnrollmentResponse?> UpdateAsync(
        Guid id,
        UpdateEnrollmentRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync(
            $"api/enrollments/{id}",
            request);

        if (response.StatusCode ==
            System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content
            .ReadFromJsonAsync<EnrollmentResponse>();
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync(
            $"api/enrollments/{id}");

        if (response.StatusCode ==
            System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }

        response.EnsureSuccessStatusCode();

        return true;
    }
}