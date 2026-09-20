using System.Net.Http.Json;
using AcademyERP.Admin.Models.FeePaymentSubmissions;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Headers;


namespace AcademyERP.Admin.Services;

public class FeePaymentSubmissionApiService
{
    private readonly HttpClient _httpClient;

    public FeePaymentSubmissionApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<FeePaymentSubmissionResponse>> GetAllAsync()
    {
        return await _httpClient.GetFromJsonAsync<
            List<FeePaymentSubmissionResponse>>(
                "api/fee-payment-submissions")
            ?? new();
    }

    public async Task<List<FeePaymentSubmissionResponse>> GetMyAsync()
    {
        return await _httpClient.GetFromJsonAsync<
            List<FeePaymentSubmissionResponse>>(
                "api/fee-payment-submissions/my")
            ?? new();
    }

    public async Task<FeePaymentSubmissionResponse?> GetByIdAsync(
        Guid id)
    {
        return await _httpClient.GetFromJsonAsync<
            FeePaymentSubmissionResponse>(
                $"api/fee-payment-submissions/{id}");
    }

    public async Task<FeePaymentSubmissionResponse?> SubmitAsync(
        CreateFeePaymentSubmissionRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "api/fee-payment-submissions",
            request);

        if (!response.IsSuccessStatusCode)
        {
            var message =
                await response.Content.ReadAsStringAsync();

            throw new InvalidOperationException(
                string.IsNullOrWhiteSpace(message)
                    ? "Unable to submit payment."
                    : message.Trim('"'));
        }

        return await response.Content.ReadFromJsonAsync<
            FeePaymentSubmissionResponse>();
    }

    public async Task<FeePaymentSubmissionResponse?> ReviewAsync(
        Guid id,
        ReviewFeePaymentSubmissionRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync(
            $"api/fee-payment-submissions/{id}/review",
            request);

        if (!response.IsSuccessStatusCode)
        {
            var message =
                await response.Content.ReadAsStringAsync();

            throw new InvalidOperationException(
                string.IsNullOrWhiteSpace(message)
                    ? "Unable to review payment."
                    : message.Trim('"'));
        }

        return await response.Content.ReadFromJsonAsync<
            FeePaymentSubmissionResponse>();
    }

    public async Task<FeePaymentSubmissionResponse?> UploadProofAsync(
    Guid submissionId,
    IBrowserFile file)
{
    await using var stream =
        file.OpenReadStream(5_242_880);

    using var content =
        new MultipartFormDataContent();

    using var fileContent =
        new StreamContent(stream);

    fileContent.Headers.ContentType =
        new MediaTypeHeaderValue(
            file.ContentType);

    content.Add(
        fileContent,
        "file",
        file.Name);

    var response =
        await _httpClient.PostAsync(
            $"api/fee-payment-submissions/{submissionId}/proof",
            content);

    if (!response.IsSuccessStatusCode)
    {
        var message =
            await response.Content.ReadAsStringAsync();

        throw new InvalidOperationException(
            string.IsNullOrWhiteSpace(message)
                ? "Unable to upload payment proof."
                : message.Trim('"'));
    }

    return await response.Content
        .ReadFromJsonAsync<FeePaymentSubmissionResponse>();
}

public async Task<byte[]> GetProofAsync(Guid id)
{
    var response = await _httpClient.GetAsync(
        $"api/fee-payment-submissions/{id}/proof");

    if (!response.IsSuccessStatusCode)
    {
        var message = await response.Content.ReadAsStringAsync();

        throw new InvalidOperationException(
            string.IsNullOrWhiteSpace(message)
                ? "Unable to load payment proof."
                : message.Trim('"'));
    }

    return await response.Content.ReadAsByteArrayAsync();
}
}