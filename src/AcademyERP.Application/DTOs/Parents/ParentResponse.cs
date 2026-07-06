using AcademyERP.Domain.Enums;

namespace AcademyERP.Application.DTOs.Parents;

public class ParentResponse
{
    public Guid Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? Remarks { get; set; }

    public UserStatus Status { get; set; }
}