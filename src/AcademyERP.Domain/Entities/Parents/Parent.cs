using AcademyERP.Domain.Entities.Common;
using AcademyERP.Domain.Entities.StudentParents;
using AcademyERP.Domain.Entities.Identity;
using AcademyERP.Domain.Enums;

namespace AcademyERP.Domain.Entities.Parents;

public class Parent : BaseEntity
{
    public Guid ApplicationUserId { get; set; }

    public ApplicationUser ApplicationUser { get; set; } = null!;

    public string ParentNumber { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public string TimeZone { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string Occupation { get; set; } = string.Empty;

    public string? Remarks { get; set; }

    public UserStatus Status { get; set; } = UserStatus.Active;

    public ICollection<StudentParent> StudentParents { get; set; }
        = new List<StudentParent>();
        
}