using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
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

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        await EnsureSuccessAsync(response);

        return await response.Content
            .ReadFromJsonAsync<EnrollmentResponse>();
    }

    public async Task<EnrollmentResponse?> CreateAsync(
        CreateEnrollmentRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "api/enrollments",
            request);

        await EnsureSuccessAsync(response);

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

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        await EnsureSuccessAsync(response);

        return await response.Content
            .ReadFromJsonAsync<EnrollmentResponse>();
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync(
            $"api/enrollments/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }

        await EnsureSuccessAsync(response);

        return true;
    }

    private static async Task EnsureSuccessAsync(
        HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var responseBody =
            await response.Content.ReadAsStringAsync();

        string? message = null;

        if (!string.IsNullOrWhiteSpace(responseBody))
        {
            try
            {
                var error =
                    JsonSerializer.Deserialize<ApiErrorResponse>(
                        responseBody,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                message = error?.Message;
            }
            catch (JsonException)
            {
                // Ignore invalid error JSON and use the HTTP message.
            }
        }

        throw new HttpRequestException(
            message
            ?? $"Request failed with status code {(int)response.StatusCode} " +
               $"({response.StatusCode}).");
    }

    private sealed class ApiErrorResponse
    {
        public string? Message { get; set; }
    }
}
