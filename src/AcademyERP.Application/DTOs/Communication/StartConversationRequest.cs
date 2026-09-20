namespace AcademyERP.Application.DTOs.Communication;

public class StartConversationRequest
{
    public Guid? StudentId { get; set; }

    public Guid ReceiverUserId { get; set; }

    public string MessageText { get; set; } = "";
}