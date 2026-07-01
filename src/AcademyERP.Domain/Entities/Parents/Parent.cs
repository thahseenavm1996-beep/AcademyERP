using AcademyERP.Domain.Entities.Common;
using AcademyERP.Domain.Enums;

namespace AcademyERP.Domain.Entities.Parents;

public class Parent : BaseEntity
{
    public Guid ApplicationUserId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? Remarks { get; set; }

    public UserStatus Status { get; set; } = UserStatus.Active;
}