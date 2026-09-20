namespace AcademyERP.Admin.Models.Communication;

public class SendMessageRequest
{
    public Guid ConversationId { get; set; }

    public Guid ReceiverUserId { get; set; }

    public string MessageText { get; set; } = "";
}