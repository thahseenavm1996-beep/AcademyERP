using System.Net.Http.Json;
using AcademyERP.Admin.Models.Registration;

namespace AcademyERP.Admin.Services;

public class RegistrationRequestApiService
{
    private readonly HttpClient _httpClient;

    public RegistrationRequestApiService(
        HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<RegistrationRequestResponse>>
        GetAllAsync()
    {
        return await _httpClient
            .GetFromJsonAsync<
                List<RegistrationRequestResponse>>(
                "api/RegistrationRequests")
            ?? new();
    }
    public async Task<RegistrationRequestResponse?>
    GetByIdAsync(Guid id)
{
    return await _httpClient
        .GetFromJsonAsync<
            RegistrationRequestResponse>(
            $"api/RegistrationRequests/{id}");
}
public async Task<bool> ApproveAsync(
    Guid requestId,
    List<StudentApprovalModel> students)
{
    var request =
        new
        {
            RegistrationRequestId = requestId,
            Students = students.Select(x => new
            {
                RegistrationStudentId =
                    x.RegistrationStudentId,

                CourseId =
                    x.CourseId,

                TeacherId =
                    x.TeacherId,

                TimeSlotId =
                    x.TimeSlotId,

                ClassDurationId =
                    x.ClassDurationId
            })
        };

    var response =
        await _httpClient.PostAsJsonAsync(
            "api/RegistrationRequests/approve",
            request);

    response.EnsureSuccessStatusCode();

    return true;
}
public async Task<bool> RejectAsync(
    Guid id,
    string remarks)
{
    var response =
        await _httpClient.PostAsJsonAsync(
            $"api/RegistrationRequests/{id}/reject",
            remarks);

    response.EnsureSuccessStatusCode();

    return true;
}
}