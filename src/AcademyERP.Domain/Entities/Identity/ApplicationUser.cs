using AcademyERP.Domain.Entities.Common;

namespace AcademyERP.Domain.Entities.Identity;

public class ApplicationUser : BaseEntity
{
    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}