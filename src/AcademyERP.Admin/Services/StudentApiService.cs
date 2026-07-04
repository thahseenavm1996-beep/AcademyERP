using System.Net.Http.Json;
using AcademyERP.Admin.Models;

namespace AcademyERP.Admin.Services;

public class StudentApiService
{
    private readonly HttpClient _httpClient;

    public StudentApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<StudentListResponse?> GetStudentsAsync(StudentQueryRequest request)
    {
        var response =
            await _httpClient.GetFromJsonAsync<StudentListResponse>(
    $"api/Students?page={request.Page}&pageSize={request.PageSize}&search={request.Search}");

        return response;
    }
    public async Task<ApiResult<StudentResponse>> CreateStudentAsync(CreateStudentRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Students", request);

        if (!response.IsSuccessStatusCode)
        {
            var validation =
                await response.Content.ReadFromJsonAsync<ValidationErrorResponse>();

            if (validation?.Errors?.Any() == true)
            {
                return new ApiResult<StudentResponse>
                {
                    Success = false,
                    Errors = validation.Errors
                        .SelectMany(x => x.Value)
                        .ToList()
                };
            }

            return new ApiResult<StudentResponse>
            {
                Success = false,
                Errors = new List<string>
        {
            "Unable to create student."
        }
            };
        }
        var result =
            await response.Content.ReadFromJsonAsync<ApiResponse<StudentResponse>>();

        return new ApiResult<StudentResponse>
        {
            Success = true,
            Data = result!.Data
        };
    }
    public async Task<ApiResult<StudentResponse>> UpdateStudentAsync(
    Guid id,
    AcademyERP.Admin.Models.UpdateStudentRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/Students/{id}", request);

        if (!response.IsSuccessStatusCode)
        {
            var validation =
                await response.Content.ReadFromJsonAsync<ValidationErrorResponse>();

            if (validation?.Errors?.Any() == true)
            {
                return new ApiResult<StudentResponse>
                {
                    Success = false,
                    Errors = validation.Errors
                        .SelectMany(x => x.Value)
                        .ToList()
                };
            }

            return new ApiResult<StudentResponse>
            {
                Success = false,
                Errors = new()
            {
                "Unable to update student."
            }
            };
        }

        var result =
            await response.Content.ReadFromJsonAsync<ApiResponse<StudentResponse>>();

        return new ApiResult<StudentResponse>
        {
            Success = true,
            Data = result!.Data
        };
    }
    public async Task<bool> DeleteStudentAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/Students/{id}");

        return response.IsSuccessStatusCode;
    }
}