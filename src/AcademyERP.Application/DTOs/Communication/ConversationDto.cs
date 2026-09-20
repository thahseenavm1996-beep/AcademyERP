namespace AcademyERP.Application.DTOs.Communication;

public class ConversationDto
{
    public Guid Id { get; set; }

    public Guid? StudentId { get; set; }

    public string StudentName { get; set; } = "";

    public string OtherUserName { get; set; } = "";

    public string LastMessage { get; set; } = "";

    public int UnreadCount { get; set; }

    public DateTime LastMessageAt { get; set; }
    public Guid OtherUserId { get; set; }


}