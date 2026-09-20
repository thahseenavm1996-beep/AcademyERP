using System.Net.Http.Json;
using AcademyERP.Admin.Models;

namespace AcademyERP.Admin.Services;

public class AuthApiService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AuthApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var http = _httpClientFactory.CreateClient("AcademyERPAuthApi");

        var response = await http.PostAsJsonAsync(
            "api/auth/login",
            request);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content
            .ReadFromJsonAsync<LoginResponse>();
    }
}