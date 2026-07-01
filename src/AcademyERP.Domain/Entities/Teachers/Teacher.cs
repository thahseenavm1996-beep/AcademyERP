using AcademyERP.Domain.Entities.Common;
using AcademyERP.Domain.Enums;

namespace AcademyERP.Domain.Entities.Teachers;

public class Teacher : BaseEntity
{
    public Guid ApplicationUserId { get; set; }

    public string EmployeeCode { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public Gender Gender { get; set; }

    public string Country { get; set; } = string.Empty;

    public string TimeZone { get; set; } = string.Empty;

    public string? Remarks { get; set; }

    public UserStatus Status { get; set; } = UserStatus.Active;
}