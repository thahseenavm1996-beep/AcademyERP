using AcademyERP.Domain.Enums;
namespace AcademyERP.Admin.Models.Registration;

public class RegistrationRequestResponse
{
    public Guid Id { get; set; }

    public string ParentName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public string? Address { get; set; }

   public int Status { get; set; }

    public DateTime SubmittedAt { get; set; }

    public List<RegistrationStudentResponse>
        Students { get; set; } = new();
}