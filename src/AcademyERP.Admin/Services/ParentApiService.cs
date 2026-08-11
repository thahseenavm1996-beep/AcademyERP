using System.Net.Http.Json;
using AcademyERP.Admin.Models;

namespace AcademyERP.Admin.Services;

public class ParentApiService
{
    private readonly HttpClient _httpClient;

    public ParentApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /* public async Task<ParentListResponse?> GetParentsAsync(ParentQueryRequest request)
     {
         return await _httpClient.GetFromJsonAsync<ParentListResponse>(
             $"api/Parents?page={request.Page}&pageSize={request.PageSize}&search={request.Search}");
     }*/
    public async Task<ParentListResponse?> GetParentsAsync(ParentQueryRequest request)
    {
        var response = await _httpClient.GetAsync(
            $"api/Parents?page={request.Page}&pageSize={request.PageSize}&search={request.Search}");

        Console.WriteLine($"Status Code: {response.StatusCode}");

        var body = await response.Content.ReadAsStringAsync();

        Console.WriteLine(body);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<ParentListResponse>();
    }

    public async Task<ApiResult<ParentResponse>> CreateParentAsync(CreateParentRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Parents", request);

        if (!response.IsSuccessStatusCode)
        {
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();

                Console.WriteLine("============== API ERROR ==============");
                Console.WriteLine(body);
                Console.WriteLine("=======================================");

                return new ApiResult<ParentResponse>
                {
                    Success = false,
                    Errors = new List<string> { body }
                };
            }
        }

        var result =
            await response.Content.ReadFromJsonAsync<ApiResponse<ParentResponse>>();

        return new ApiResult<ParentResponse>
        {
            Success = true,
            Data = result!.Data
        };
    }

    public async Task<ApiResult<ParentResponse>> UpdateParentAsync(
        Guid id,
        UpdateParentRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/Parents/{id}", request);

        if (!response.IsSuccessStatusCode)
        {
            var validation =
                await response.Content.ReadFromJsonAsync<ValidationErrorResponse>();

            if (validation?.Errors?.Any() == true)
            {
                return new ApiResult<ParentResponse>
                {
                    Success = false,
                    Errors = validation.Errors
                        .SelectMany(x => x.Value)
                        .ToList()
                };
            }

            return new ApiResult<ParentResponse>
            {
                Success = false,
                Errors = new()
                {
                    "Unable to update parent."
                }
            };
        }

        var result =
            await response.Content.ReadFromJsonAsync<ApiResponse<ParentResponse>>();

        return new ApiResult<ParentResponse>
        {
            Success = true,
            Data = result!.Data
        };
    }

    public async Task<bool> DeleteParentAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/Parents/{id}");

        return response.IsSuccessStatusCode;
    }

    public async Task<ParentResponse?> GetParentAsync(Guid id)
    {
        var response =
            await _httpClient.GetFromJsonAsync<ApiResponse<ParentResponse>>(
                $"api/Parents/{id}");

        return response?.Data;
    }
    public async Task<ParentListResponse?> GetParentsAsync()
    {
        return await GetParentsAsync(new ParentQueryRequest
        {
            Page = 1,
            PageSize = 100,
            Search = ""
        });
    }
    public async Task<bool> ResetPasswordAsync(Guid id, ResetParentPasswordRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(
            $"api/Parents/{id}/reset-password",
            request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();

            throw new Exception(error);
        }

        return true;
    }

}