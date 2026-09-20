using System.Net.Http.Json;
using AcademyERP.Admin.Models.Reports;

namespace AcademyERP.Admin.Services;

public class BestPerformingFacultyApiService
{
    private readonly HttpClient _http;

    public BestPerformingFacultyApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<BestPerformingFacultyReportResponse?> GetAsync(
        string? courseName,
        int? gender,
        DateOnly? fromDate,
        DateOnly? toDate)
    {
        var query = BuildQuery(
            courseName,
            gender,
            fromDate,
            toDate);

        return await _http.GetFromJsonAsync<
            BestPerformingFacultyReportResponse>(
                "api/reports/best-performing-faculty" + query);
    }

    public async Task<byte[]> DownloadImageAsync(
        string? courseName,
        int? gender,
        DateOnly? fromDate,
        DateOnly? toDate)
    {
        return await _http.GetByteArrayAsync(
            "api/reports/best-performing-faculty/image" +
            BuildQuery(
                courseName,
                gender,
                fromDate,
                toDate));
    }

    public async Task<byte[]> DownloadPdfAsync(
        string? courseName,
        int? gender,
        DateOnly? fromDate,
        DateOnly? toDate)
    {
        return await _http.GetByteArrayAsync(
            "api/reports/best-performing-faculty/pdf" +
            BuildQuery(
                courseName,
                gender,
                fromDate,
                toDate));
    }

    private static string BuildQuery(
        string? courseName,
        int? gender,
        DateOnly? fromDate,
        DateOnly? toDate)
    {
        var query = new List<string>();

        if (!string.IsNullOrWhiteSpace(courseName))
        {
            query.Add(
                $"courseName={Uri.EscapeDataString(courseName)}");
        }

        if (gender.HasValue)
        {
            query.Add($"gender={gender.Value}");
        }

        if (fromDate.HasValue)
        {
            query.Add(
                $"fromDate={fromDate:yyyy-MM-dd}");
        }

        if (toDate.HasValue)
        {
            query.Add(
                $"toDate={toDate:yyyy-MM-dd}");
        }

        return query.Count == 0
            ? string.Empty
            : "?" + string.Join("&", query);
    }
}
