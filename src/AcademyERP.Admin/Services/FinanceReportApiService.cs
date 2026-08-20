using System.Net.Http.Json;
using AcademyERP.Admin.Models.FinanceReports;

namespace AcademyERP.Admin.Services;

public class FinanceReportApiService
{
    private readonly HttpClient _http;


    public FinanceReportApiService(HttpClient http)
    {
        _http = http;
    }


    public async Task<FinanceReportDashboardResponse?> GetAsync()
    {
        return await _http.GetFromJsonAsync<FinanceReportDashboardResponse>(
            "api/reports/finance");
    }
}