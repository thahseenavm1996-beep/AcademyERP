using System.Net.Http.Json;
using AcademyERP.Admin.Models;

namespace AcademyERP.Admin.Services;

public class TeacherApiService
{
    private readonly HttpClient _httpClient;

    public TeacherApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<TeacherListResponse?> GetTeachersAsync(TeacherQueryRequest request)
    {
        var response =
            await _httpClient.GetFromJsonAsync<TeacherListResponse>(
    $"api/Teachers?page={request.Page}&pageSize={request.PageSize}&search={request.Search}");

        return response;
    }
    public async Task<ApiResult<TeacherResponse>> CreateTeacherAsync(CreateTeacherRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Teachers", request);

        var body = await response.Content.ReadAsStringAsync();

        Console.WriteLine($"Status: {response.StatusCode}");
        Console.WriteLine(body);

        if (!response.IsSuccessStatusCode)
        {
            var validation =
                await response.Content.ReadFromJsonAsync<ValidationErrorResponse>();

            if (validation?.Errors?.Any() == true)
            {
                return new ApiResult<TeacherResponse>
                {
                    Success = false,
                    Errors = validation.Errors
                        .SelectMany(x => x.Value)
                        .ToList()
                };
            }

            return new ApiResult<TeacherResponse>
            {
                Success = false,
                Errors = new List<string>
        {
            "Unable to create teacher."
        }
            };
        }
        var result =
            await response.Content.ReadFromJsonAsync<ApiResponse<TeacherResponse>>();

        return new ApiResult<TeacherResponse>
        {
            Success = true,
            Data = result!.Data
        };
    }
    public async Task<ApiResult<TeacherResponse>> UpdateTeacherAsync(
    Guid id,
    AcademyERP.Admin.Models.UpdateTeacherRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/Teachers/{id}", request);

        if (!response.IsSuccessStatusCode)
        {
            var validation =
                await response.Content.ReadFromJsonAsync<ValidationErrorResponse>();

            if (validation?.Errors?.Any() == true)
            {
                return new ApiResult<TeacherResponse>
                {
                    Success = false,
                    Errors = validation.Errors
                        .SelectMany(x => x.Value)
                        .ToList()
                };
            }

            return new ApiResult<TeacherResponse>
            {
                Success = false,
                Errors = new()
            {
                "Unable to update teacher."
            }
            };
        }

        var result =
            await response.Content.ReadFromJsonAsync<ApiResponse<TeacherResponse>>();

        return new ApiResult<TeacherResponse>
        {
            Success = true,
            Data = result!.Data
        };
    }
    public async Task<bool> DeleteTeacherAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/Teachers/{id}");

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> ResetPasswordAsync(Guid id, ResetTeacherPasswordRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(
            $"api/Teachers/{id}/reset-password",
            request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();

            throw new Exception(error);
        }

        return true;
    }
}