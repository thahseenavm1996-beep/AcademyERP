using System.Net.Http.Json;
using AcademyERP.Admin.Models.TeacherDashboard;

namespace AcademyERP.Admin.Services;

public class TeacherDashboardApiService
{
    private readonly HttpClient _http;

    public TeacherDashboardApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<TeacherDashboardResponse?> GetDashboardAsync(Guid teacherId)
    {
        return await _http.GetFromJsonAsync<TeacherDashboardResponse>(
            $"api/teacher/dashboard/{teacherId}");
    }
}