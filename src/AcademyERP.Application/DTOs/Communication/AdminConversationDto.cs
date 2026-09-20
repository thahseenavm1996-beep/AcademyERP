namespace AcademyERP.Application.DTOs.Communication;

public class AdminConversationDto
{
    public Guid ConversationId { get; set; }

    public Guid? ParentId { get; set; }

    public string ParentName { get; set; } = "";

    public Guid? StudentId { get; set; }

    public string StudentName { get; set; } = "";

    public Guid? TeacherId { get; set; }

    public string TeacherName { get; set; } = "";

    public string LastMessage { get; set; } = "";

    public DateTime LastMessageAt { get; set; }

    public bool HasUnreadMessages { get; set; }
}