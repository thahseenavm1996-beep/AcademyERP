namespace AcademyERP.Application.DTOs.Communication;

public class StartTeacherConversationRequest
{
    public Guid ReceiverUserId { get; set; }

    public Guid? StudentId { get; set; }

    public string MessageText { get; set; } = "";
}