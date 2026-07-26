using System.Net.Http.Json;
using AcademyERP.Admin.Models.Lookups;

namespace AcademyERP.Admin.Services;

public class LookupApiService
{
    private readonly HttpClient _httpClient;

    public LookupApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<TimeSlotResponse>> GetTimeSlotsAsync()
    {
        return await _httpClient
            .GetFromJsonAsync<List<TimeSlotResponse>>(
                "api/lookups/timeslots")
            ?? new List<TimeSlotResponse>();
    }

    public async Task<List<ClassDurationResponse>> GetClassDurationsAsync()
    {
        return await _httpClient
            .GetFromJsonAsync<List<ClassDurationResponse>>(
                "api/lookups/classdurations")
            ?? new List<ClassDurationResponse>();
    }
}