using AcademyERP.Admin.Models;

namespace AcademyERP.Admin.Services;

public class DashboardApiService
{
    private readonly HttpClient _http;

    public DashboardApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<DashboardSummaryResponse?> GetSummaryAsync()
    {
        return await _http.GetFromJsonAsync<DashboardSummaryResponse>(
            "api/Dashboard");
    }
}