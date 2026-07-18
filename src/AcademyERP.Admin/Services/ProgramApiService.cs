using AcademyERP.Application.DTOs.Programs;
using System.Net.Http.Json;

namespace AcademyERP.Admin.Services;

public class ProgramApiService
{
    private readonly HttpClient _http;

    public ProgramApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<ProgramResponse>> GetAllAsync()
    {
        return await _http.GetFromJsonAsync<List<ProgramResponse>>("api/Programs")
               ?? new();
    }

    public async Task<ProgramResponse?> GetByIdAsync(Guid id)
    {
        return await _http.GetFromJsonAsync<ProgramResponse>($"api/Programs/{id}");
    }

    public async Task<bool> CreateAsync(CreateProgramRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/Programs", request);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateProgramRequest request)
    {
        var response = await _http.PutAsJsonAsync($"api/Programs/{id}", request);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var response = await _http.DeleteAsync($"api/Programs/{id}");
        return response.IsSuccessStatusCode;
    }
}