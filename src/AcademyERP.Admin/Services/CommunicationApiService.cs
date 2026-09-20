using System.Net.Http.Json;
using AcademyERP.Admin.Models.Communication;
using MessageRecipientDto =
    AcademyERP.Application.DTOs.Communication.MessageRecipientDto;
using StartConversationRequest =
    AcademyERP.Application.DTOs.Communication.StartConversationRequest;
    using AdminConversationDto =
    AcademyERP.Application.DTOs.Communication.AdminConversationDto;
    using TeacherMessageRecipientDto =
    AcademyERP.Application.DTOs.Communication.TeacherMessageRecipientDto;

using StartTeacherConversationRequest =
    AcademyERP.Application.DTOs.Communication.StartTeacherConversationRequest;

namespace AcademyERP.Admin.Services;

public class CommunicationApiService
{
    private readonly HttpClient _http;


    public CommunicationApiService(
        HttpClient http)
    {
        _http = http;
    }



    // Admin
    public async Task<List<ConversationDto>> GetAdminConversationsAsync()
    {
        return await _http.GetFromJsonAsync<List<ConversationDto>>(
            "api/admin/messages/all")
            ?? [];
    }

public async Task<List<AdminConversationDto>>
    GetAdminMonitoredConversationsAsync()
{
    return await _http.GetFromJsonAsync<List<AdminConversationDto>>(
        "api/admin/messages/monitor")
        ?? [];
}

    // Teacher
    public async Task<List<ConversationDto>> GetTeacherConversationsAsync()
    {
        return await _http.GetFromJsonAsync<List<ConversationDto>>(
            "api/teacher/messages")
            ?? [];
    }



    // Parent
    public async Task<List<ConversationDto>> GetParentConversationsAsync()
    {
        return await _http.GetFromJsonAsync<List<ConversationDto>>(
            "api/parent/messages")
            ?? [];
    }



    public async Task<List<MessageDto>> GetMessagesAsync(
        Guid conversationId,
        string role = "admin")
    {

        string url = role switch
        {
            "teacher" =>
            $"api/teacher/messages/{conversationId}",

            "parent" =>
            $"api/parent/messages/{conversationId}",

            _ =>
            $"api/admin/messages/{conversationId}"
        };


        return await _http.GetFromJsonAsync<List<MessageDto>>(url)
            ?? [];
    }




    public async Task SendAsync(
        SendMessageRequest request,
        string role = "admin")
    {

        string url = role switch
        {
            "teacher" =>
            "api/teacher/messages",

            "parent" =>
            "api/parent/messages",

            _ =>
            "api/admin/messages"
        };


        await _http.PostAsJsonAsync(
            url,
            request);
    }
    public async Task<List<MessageRecipientDto>>
    GetTeacherRecipientsAsync()
{
    return await _http.GetFromJsonAsync<
        List<MessageRecipientDto>>(
            "api/admin/messages/teachers")
        ?? [];
}

public async Task<Guid> StartConversationAsync(
    StartConversationRequest request)
{
    var response =
        await _http.PostAsJsonAsync(
            "api/admin/messages/start",
            request);

    response.EnsureSuccessStatusCode();

    return await response.Content
        .ReadFromJsonAsync<Guid>();
}

public async Task<List<MessageRecipientDto>> GetParentTeacherRecipientsAsync()
{
    return await _http.GetFromJsonAsync<List<MessageRecipientDto>>(
        "api/parent/messages/recipients")
        ?? [];
}
public async Task<Guid> StartParentConversationAsync(
    StartConversationRequest request)
{
    var response =
        await _http.PostAsJsonAsync(
            "api/parent/messages/start",
            request);

    response.EnsureSuccessStatusCode();

    return await response.Content
        .ReadFromJsonAsync<Guid>();
}

public async Task<List<MessageDto>>
    GetAdminMonitoredMessagesAsync(
        Guid conversationId)
{
    return await _http.GetFromJsonAsync<List<MessageDto>>(
        $"api/admin/messages/monitor/{conversationId}")
        ?? [];
}

public async Task<int> GetAdminUnreadMessageCountAsync()
{
    return await _http.GetFromJsonAsync<int>(
        "api/admin/messages/unread-count");
}

public async Task<List<TeacherMessageRecipientDto>> GetTeacherMessageRecipientsAsync()
{
    return await _http.GetFromJsonAsync<List<TeacherMessageRecipientDto>>(
        "api/teacher/messages/recipients") ?? [];
}

public async Task<Guid> StartTeacherConversationAsync(
    StartTeacherConversationRequest request)
{
    var response = await _http.PostAsJsonAsync(
        "api/teacher/messages/start",
        request);

    response.EnsureSuccessStatusCode();

    return await response.Content.ReadFromJsonAsync<Guid>();
}

}