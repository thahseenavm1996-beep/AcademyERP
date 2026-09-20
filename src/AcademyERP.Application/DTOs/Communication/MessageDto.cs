namespace AcademyERP.Application.DTOs.Communication;

public class MessageDto
{
    public Guid Id { get; set; }

    public string SenderName { get; set; } = "";

    public string MessageText { get; set; } = "";

    public DateTime SentAt { get; set; }

    public bool IsMine { get; set; }
}