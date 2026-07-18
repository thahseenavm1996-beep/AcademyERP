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
        Console.WriteLine("===== STUDENTS ENDPOINT HIT =====");
        var response = await _httpClient.GetAsync(
            $"api/Students?page={request.Page}&pageSize={request.PageSize}&search={request.Search}");

        Console.WriteLine("STATUS = " + response.StatusCode);

        Console.WriteLine("AUTH HEADER = " +
            response.RequestMessage?.Headers.Authorization);

        var body = await response.Content.ReadAsStringAsync();

        Console.WriteLine(body);

        response.EnsureSuccessStatusCode();

        return System.Text.Json.JsonSerializer.Deserialize<StudentListResponse>(
            body,
            new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
    }
    /* public async Task<StudentListResponse?> GetStudentsAsync(StudentQueryRequest request)
     {
         /* var response =
              await _httpClient.GetFromJsonAsync<StudentListResponse>(
      $"api/Students?page={request.Page}&pageSize={request.PageSize}&search={request.Search}");
       return response;
         var response = await _httpClient.GetAsync(
         $"api/Students?page={request.Page}&pageSize={request.PageSize}&search={request.Search}");

         Console.WriteLine($"Status: {response.StatusCode}");

         Console.WriteLine("Authorization header:");
         Console.WriteLine(response.RequestMessage?.Headers.Authorization);

         var body = await response.Content.ReadAsStringAsync();

         Console.WriteLine(body);

         response.EnsureSuccessStatusCode();

         return System.Text.Json.JsonSerializer.Deserialize<StudentListResponse>(
             body,
             new System.Text.Json.JsonSerializerOptions
             {
                 PropertyNameCaseInsensitive = true
             });


     }*/
    public async Task<StudentResponse?> GetStudentAsync(Guid id)
    {
        var response = await _httpClient.GetAsync($"api/Students/{id}");

        if (!response.IsSuccessStatusCode)
            return null;

        var result =
            await response.Content.ReadFromJsonAsync<ApiResponse<StudentResponse>>();

        return result?.Data;
    }
    public async Task<ApiResult<StudentResponse>> CreateStudentAsync(CreateStudentRequest request)
    {
        var apiRequest = new
        {
            FullName = request.FullName,
            AdmissionDate = request.AdmissionDate!.Value,
            DateOfBirth = request.DateOfBirth!.Value,
            Gender = request.Gender,
            PhoneNumber = request.MobileNumber,
            Email = request.Email,
            Country = request.Country,
            TimeZone = request.TimeZone,
            Password = request.Password,
            Remarks = request.Remarks
        };

        var response = await _httpClient.PostAsJsonAsync("api/Students", apiRequest);

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
    public async Task<bool> ResetPasswordAsync(Guid id, ResetStudentPasswordRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(
            $"api/Students/{id}/reset-password",
            request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();

            throw new Exception(error);
        }

        return true;
    }
}