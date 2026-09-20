using AcademyERP.Application.DTOs.Communication;

namespace AcademyERP.Application.Services;

public interface ICommunicationService
{
    Task<List<ConversationDto>> GetMyConversationsAsync(
        Guid userId);

    Task<List<MessageDto>> GetMessagesAsync(
        Guid conversationId,
        Guid userId);
Task<List<MessageDto>> GetAdminMessagesAsync(
    Guid conversationId,
    Guid adminUserId);

    Task SendMessageAsync(
        Guid senderUserId,
        SendMessageRequest request);

    Task<List<ConversationDto>> GetAllConversationsAsync(
    Guid userId);

    Task<List<MessageRecipientDto>> GetTeacherRecipientsAsync();
    Task<List<MessageRecipientDto>> GetParentTeacherRecipientsAsync(
    Guid parentUserId);

    Task<Guid> StartConversationAsync(
    Guid senderUserId,
    Guid receiverUserId,
    string messageText,
    Guid? studentId = null);

Task<List<TeacherMessageRecipientDto>> GetTeacherMessageRecipientsAsync(Guid teacherUserId);

Task<Guid> StartTeacherConversationAsync(
    Guid teacherUserId,
    Guid receiverUserId,
    string messageText,
    Guid? studentId = null);

    Task<List<AdminConversationDto>> GetAdminMonitoredConversationsAsync();

    Task<List<MessageDto>> GetAdminMonitoredMessagesAsync(
    Guid conversationId);

    Task<int> GetAdminUnreadMessageCountAsync(Guid userId);
}