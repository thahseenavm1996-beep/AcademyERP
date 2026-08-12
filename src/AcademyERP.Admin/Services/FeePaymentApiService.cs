using System.Net.Http.Json;
using AcademyERP.Admin.Models.Payments;

namespace AcademyERP.Admin.Services;

public class FeePaymentApiService
{
    private readonly HttpClient _httpClient;

    public FeePaymentApiService(
        HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<FeePaymentResponse>>
    GetAllAsync()
{
    var response =
        await _httpClient.GetFromJsonAsync<
            ApiResponse<PagedResponse<FeePaymentResponse>>>(
            "api/FeePayments");

    return response?.Data?.Items
           ?? new List<FeePaymentResponse>();
}

    public async Task<FeePaymentResponse?>
        CreateAsync(
            CreateFeePaymentRequest request)
    {
        var response =
            await _httpClient.PostAsJsonAsync(
                "api/FeePayments",
                request);

        response.EnsureSuccessStatusCode();

        var result =
            await response.Content
                .ReadFromJsonAsync<
                    ApiResponse<FeePaymentResponse>>();

        return result?.Data;
    }

    private class ApiResponse<T>
    {
        public bool Success { get; set; }

        public string Message { get; set; }
            = string.Empty;

        public T? Data { get; set; }
    }
    private class PagedResponse<T>
{
    public List<T> Items { get; set; } = new();

    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public int TotalPages { get; set; }
}
}