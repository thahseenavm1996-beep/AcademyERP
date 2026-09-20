using System.Net.Http.Json;
using AcademyERP.Admin.Models.ReportUnlockRequests;
using AcademyERP.Domain.Enums;

namespace AcademyERP.Admin.Services;

public class ReportUnlockRequestApiService
{
    private readonly HttpClient _http;

    public ReportUnlockRequestApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<ReportUnlockRequestResponse>>
        GetAllAsync(ReportUnlockRequestStatus? status = null)
    {
        var url = "api/report-unlock-requests";

        if (status.HasValue)
        {
            url += $"?status={Uri.EscapeDataString(status.Value.ToString())}";
        }

        return await _http.GetFromJsonAsync<
            List<ReportUnlockRequestResponse>>(url)
            ?? new List<ReportUnlockRequestResponse>();
    }

    public async Task<bool> ReviewAsync(
        Guid requestId,
        ReportUnlockRequestStatus status,
        string? adminRemarks = null)
    {
        var response = await _http.PutAsJsonAsync(
            $"api/report-unlock-requests/{requestId}/review",
            new ReviewRequest
            {
                Status = status,
                AdminRemarks = adminRemarks
            });

        return response.IsSuccessStatusCode;
    }

    private sealed class ReviewRequest
    {
        public ReportUnlockRequestStatus Status { get; set; }

        public string? AdminRemarks { get; set; }
    }
}
