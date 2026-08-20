using System.Net.Http.Json;
using AcademyERP.Admin.Models.ParentPortal;

namespace AcademyERP.Admin.Services;

public class ParentPortalApiService
{
    private readonly HttpClient _http;


    public ParentPortalApiService(HttpClient http)
    {
        _http = http;
    }


    public async Task<ParentDashboardResponse?> GetDashboardAsync()
    {
        return await _http.GetFromJsonAsync<ParentDashboardResponse>(
            "api/parent-portal/dashboard");
    }
}