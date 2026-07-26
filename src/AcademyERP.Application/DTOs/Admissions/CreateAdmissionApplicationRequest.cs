using AcademyERP.Domain.Enums;

namespace AcademyERP.Application.DTOs.Admissions;

public class CreateAdmissionApplicationRequest
{
    public Guid ProgramId { get; set; }

    public Guid? CourseId { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public int Age { get; set; }

    public Gender Gender { get; set; }

    public string Country { get; set; } = string.Empty;

    public string? City { get; set; }

    public string WhatsAppNumber { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? PreferredTiming { get; set; }

    public string? Message { get; set; }
}