using AcademyERP.Admin.Models;
using System.Net.Http.Json;
using AcademyERP.Admin.Models.ClassReports;

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
            ApiResponse<PagedResponse<FeeInvoiceResponse>>>(
            "api/FeeInvoices");

    return response?.Data?.Items
           ?? new List<FeeInvoiceResponse>();
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
    public async Task<FeeInvoiceResponse?> GetByIdAsync(Guid id)
{
    var response =
        await _httpClient.GetFromJsonAsync<
            ApiResponse<FeeInvoiceResponse>>(
            $"api/FeeInvoices/{id}");

    return response?.Data;
}

public async Task UpdateAsync(
    Guid id,
    UpdateFeeInvoiceRequest request)
{
    await _httpClient.PutAsJsonAsync(
        $"api/FeeInvoices/{id}",
        request);
}
}