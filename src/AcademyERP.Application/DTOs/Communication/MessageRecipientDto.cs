namespace AcademyERP.Application.DTOs.Communication;

public class MessageRecipientDto
{
    public Guid UserId { get; set; }

    public Guid TeacherId { get; set; }

    public Guid StudentId { get; set; }

    public string Name { get; set; } = "";

    public string StudentName { get; set; } = "";

    public string Email { get; set; } = "";
}