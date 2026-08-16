using AcademyERP.Domain.Entities.Common;
using AcademyERP.Domain.Enums;
namespace AcademyERP.Domain.Entities.Admissions;

public class RegistrationRequest : BaseEntity
{
    public string ParentName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public string? Address { get; set; }

    public RegistrationStatus Status { get; set; }
        = RegistrationStatus.Pending;

    public DateTime SubmittedAt { get; set; }
        = DateTime.UtcNow;

    public DateTime? ReviewedAt { get; set; }

    public string? ReviewRemarks { get; set; }

    public ICollection<RegistrationRequestStudent>
        Students { get; set; }
            = new List<RegistrationRequestStudent>();
}