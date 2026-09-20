namespace AcademyERP.Application.DTOs.Communication;

public class SendMessageRequest
{
    public Guid ConversationId { get; set; }

    public Guid ReceiverUserId { get; set; }

    public string MessageText { get; set; } = "";
}