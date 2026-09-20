using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using AcademyERP.Admin.Models;

namespace AcademyERP.Admin.Services;

public class TeachingScheduleApiService
{
    private readonly HttpClient _http;

    public TeachingScheduleApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<TeachingScheduleResponse>?> GetAllAsync()
    {
        var response = await _http.GetAsync(
            "api/teachingschedules");

        Console.WriteLine(
            $"Status Code: {response.StatusCode}");

        var content =
            await response.Content.ReadAsStringAsync();

        Console.WriteLine(content);

        if (!response.IsSuccessStatusCode)
            return null;

        return JsonSerializer.Deserialize<
            List<TeachingScheduleResponse>>(
                content,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
    }

    public async Task<TeachingScheduleResponse?> CreateAsync(
        CreateTeachingScheduleRequest request)
    {
        var response = await _http.PostAsJsonAsync(
            "api/teachingschedules",
            request);

        await EnsureSuccessAsync(response);

        return await response.Content
            .ReadFromJsonAsync<TeachingScheduleResponse>();
    }

    private static async Task EnsureSuccessAsync(
        HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
            return;

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
                // Use the HTTP status message below.
            }
        }

        throw new HttpRequestException(
            message
            ?? $"Request failed with status code " +
               $"{(int)response.StatusCode} " +
               $"({response.StatusCode}).");
    }

    private sealed class ApiErrorResponse
    {
        public string? Message { get; set; }
    }
}
