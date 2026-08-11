using AcademyERP.Admin.Models;
using System.Net.Http.Json;

namespace AcademyERP.Admin.Services;

public class FeeInvoiceApiService
{
    private readonly HttpClient _httpClient;

    public FeeInvoiceApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<FeeInvoiceResponse>> GetAllAsync()
    {
       var response =
    await _httpClient.GetFromJsonAsync<
        ApiResponse<List<FeeInvoiceResponse>>>(
        "api/FeeInvoices");

return response?.Data ??
       new List<FeeInvoiceResponse>();
    }

    public async Task CreateAsync(
        CreateFeeInvoiceRequest request)
    {
        await _httpClient.PostAsJsonAsync(
            "api/FeeInvoices",
            request);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _httpClient.DeleteAsync(
            $"api/FeeInvoices/{id}");
    }
}