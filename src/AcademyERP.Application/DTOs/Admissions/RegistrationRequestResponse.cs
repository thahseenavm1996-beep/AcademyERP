using AcademyERP.Domain.Enums;

namespace AcademyERP.Application.DTOs.Admissions;

public class RegistrationRequestResponse
{
    public Guid Id { get; set; }

    public string ParentName { get; set; }
        = string.Empty;

    public string Email { get; set; }
        = string.Empty;

    public string PhoneNumber { get; set; }
        = string.Empty;

    public string Country { get; set; }
        = string.Empty;

    public RegistrationStatus Status { get; set; }

    public DateTime SubmittedAt { get; set; }

    public List<RegistrationRequestStudentResponse>
        Students { get; set; }
            = new();
}