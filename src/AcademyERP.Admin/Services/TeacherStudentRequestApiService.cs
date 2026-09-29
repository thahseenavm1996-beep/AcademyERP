using System.Net.Http.Json;
using AcademyERP.Admin.Models.TeacherStudentRequests;

namespace AcademyERP.Admin.Services;

public class TeacherStudentRequestApiService
{
    private readonly HttpClient _http;

    public TeacherStudentRequestApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<TeacherStudentRequestDto>?> GetMyRequestsAsync()
    {
        return await _http.GetFromJsonAsync<List<TeacherStudentRequestDto>>(
            "api/TeacherStudentRequests/my");
    }

    public async Task<List<TeacherStudentRequestDto>?> GetPendingAsync()
    {
        return await _http.GetFromJsonAsync<List<TeacherStudentRequestDto>>(
            "api/TeacherStudentRequests/pending");
    }

    public async Task<TeacherStudentRequestDto?> GetByIdAsync(Guid id)
    {
        return await _http.GetFromJsonAsync<TeacherStudentRequestDto>(
            $"api/TeacherStudentRequests/{id}");
    }

    public async Task<TeacherStudentRequestDto?> CreateAsync(
        CreateTeacherStudentRequestDto request)
    {
        var response = await _http.PostAsJsonAsync(
            "api/TeacherStudentRequests",
            request);

        response.EnsureSuccessStatusCode();

        return await response.Content
            .ReadFromJsonAsync<TeacherStudentRequestDto>();
    }

    public async Task<FindStudentResponseDto?> FindStudentAsync(
        string name,
        string phone)
    {
        var url =
            $"api/TeacherStudentRequests/find-student" +
            $"?name={Uri.EscapeDataString(name)}" +
            $"&phone={Uri.EscapeDataString(phone)}";

        var response = await _http.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content
            .ReadFromJsonAsync<FindStudentResponseDto>();
    }

    public async Task<TeacherStudentRequestDto?> LinkStudentAsync(
        Guid requestId,
        Guid studentId)
    {
        var response = await _http.PostAsJsonAsync(
            $"api/TeacherStudentRequests/{requestId}/link-student",
            new LinkTeacherStudentRequestDto
            {
                StudentId = studentId
            });

        response.EnsureSuccessStatusCode();

        return await response.Content
            .ReadFromJsonAsync<TeacherStudentRequestDto>();
    }

    public async Task<TeacherStudentRequestDto?> ReviewAsync(
        Guid requestId,
        ReviewTeacherStudentRequestDto request)
    {
        var response = await _http.PostAsJsonAsync(
            $"api/TeacherStudentRequests/{requestId}/review",
            request);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();

            var message = body;

            try
            {
                using var document =
                    System.Text.Json.JsonDocument.Parse(body);

                if (document.RootElement.TryGetProperty(
                        "message",
                        out var messageProperty))
                {
                    message = messageProperty.GetString()
                        ?? body;
                }
            }
            catch
            {
                // Keep the raw response body if it is not JSON.
            }

            throw new InvalidOperationException(
                string.IsNullOrWhiteSpace(message)
                    ? "Unable to review student request."
                    : message.Trim('"'));
        }

        return await response.Content
            .ReadFromJsonAsync<TeacherStudentRequestDto>();
    }
}
