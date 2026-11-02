using System.Net.Http.Json;
using AcademyERP.Admin.Models;

namespace AcademyERP.Admin.Services;

public sealed class TeamHeadUserApiService
{
    private readonly HttpClient _httpClient;

    public TeamHeadUserApiService(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<List<TeamHeadUserResponse>> GetAllAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<TeamHeadUserResponse>>(
            "api/team-head-users") ?? new();
    }

    public async Task<string?> CreateAsync(string fullName, string email, string temporaryPassword)
    {
        var response = await _httpClient.PostAsJsonAsync("api/team-head-users", new
        {
            FullName = fullName,
            Email = email,
            TemporaryPassword = temporaryPassword
        });

        if (response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string?> UpdateUsernameAsync(Guid id, string username)
    {
        var response = await _httpClient.PutAsJsonAsync(
            $"api/team-head-users/{id}/username",
            new { Username = username });
        return response.IsSuccessStatusCode
            ? null
            : await response.Content.ReadAsStringAsync();
    }

    public async Task<string?> ResetPasswordAsync(Guid id, string newPassword)
    {
        var response = await _httpClient.PutAsJsonAsync(
            $"api/team-head-users/{id}/password",
            new { NewPassword = newPassword });
        return response.IsSuccessStatusCode
            ? null
            : await response.Content.ReadAsStringAsync();
    }

    public async Task<string?> SetActiveAsync(Guid id, bool isActive)
    {
        var response = await _httpClient.PutAsJsonAsync(
            $"api/team-head-users/{id}/active",
            new { IsActive = isActive });
        return response.IsSuccessStatusCode
            ? null
            : await response.Content.ReadAsStringAsync();
    }
}
