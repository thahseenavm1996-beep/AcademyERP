using AcademyERP.Domain.Enums;

namespace AcademyERP.Admin.Models.Registration;

public class CreateRegistrationRequest
{
    public string ParentName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public string? Address { get; set; }

    public List<CreateRegistrationRequestStudent> Students { get; set; }
        = new();
}

public class CreateRegistrationRequestStudent
{
    public string StudentName { get; set; } = string.Empty;

    public DateTime DateOfBirth { get; set; } =
        DateTime.Today.AddYears(-5);

    public Gender Gender { get; set; } = Gender.Male;

    public Guid ProgramId { get; set; }

    public Guid? CourseId { get; set; }

    public string? PreferredTime { get; set; }

    public string? Remarks { get; set; }
}