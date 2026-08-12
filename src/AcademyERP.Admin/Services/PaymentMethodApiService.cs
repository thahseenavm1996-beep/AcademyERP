using System.Net.Http.Json;
using AcademyERP.Admin.Models.Payments;

namespace AcademyERP.Admin.Services;

public class PaymentMethodApiService
{
    private readonly HttpClient _httpClient;

    public PaymentMethodApiService(
        HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<PaymentMethodResponse>>
        GetAllAsync()
    {
        var response =
            await _httpClient.GetFromJsonAsync<
                ApiResponse<List<PaymentMethodResponse>>>(
                "api/PaymentMethods");

        return response?.Data ??
               new List<PaymentMethodResponse>();
    }

    private class ApiResponse<T>
    {
        public bool Success { get; set; }

        public string Message { get; set; }
            = string.Empty;

        public T? Data { get; set; }
    }
}