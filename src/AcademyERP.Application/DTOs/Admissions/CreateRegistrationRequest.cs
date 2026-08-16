namespace AcademyERP.Application.DTOs.Admissions;

public class CreateRegistrationRequest
{
    public string ParentName { get; set; }
        = string.Empty;

    public string Email { get; set; }
        = string.Empty;

    public string PhoneNumber { get; set; }
        = string.Empty;

    public string Country { get; set; }
        = string.Empty;

    public string? Address { get; set; }

    public List<CreateRegistrationRequestStudent>
        Students { get; set; }
            = new();
}