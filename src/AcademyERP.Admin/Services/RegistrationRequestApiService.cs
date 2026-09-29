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

    public async Task<RegistrationRequestResponse?>
        CreateAsync(
            CreateRegistrationRequest request)
    {
        var response =
            await _httpClient.PostAsJsonAsync(
                "api/RegistrationRequests",
                request);

        if (!response.IsSuccessStatusCode)
        {
            var error =
                await response.Content
                    .ReadAsStringAsync();

            throw new HttpRequestException(
                string.IsNullOrWhiteSpace(error)
                    ? $"Failed to create registration request. Status: {response.StatusCode}"
                    : error);
        }

        return await response.Content
            .ReadFromJsonAsync<
                RegistrationRequestResponse>();
    }

    public async Task<bool> ApproveAsync(
        Guid requestId,
        List<StudentApprovalModel> students)
    {
        var request =
            new
            {
                RegistrationRequestId =
                    requestId,

                Students =
                    students.Select(x =>
                        new
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
                                x.ClassDurationId,

                            DaysOfWeek =
    x.DaysOfWeek,

FinalStudentFee =
    x.FinalStudentFee,

TeacherPaymentAmount =
    x.TeacherPaymentAmount
                        })
            };

        var response =
            await _httpClient.PostAsJsonAsync(
                "api/RegistrationRequests/approve",
                request);

        if (!response.IsSuccessStatusCode)
        {
            var error =
                await response.Content
                    .ReadAsStringAsync();

            throw new HttpRequestException(
                string.IsNullOrWhiteSpace(error)
                    ? $"Registration approval failed. Status: {response.StatusCode}"
                    : error);
        }

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

    public async Task<bool> MarkAsViewedAsync(Guid id)
{
    var response = await _httpClient.PutAsync(
        $"api/RegistrationRequests/{id}/viewed",
        null);

    return response.IsSuccessStatusCode;
}
}