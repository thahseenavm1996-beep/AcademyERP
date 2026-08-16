using AcademyERP.Domain.Enums;

namespace AcademyERP.Application.DTOs.Admissions;

public class CreateRegistrationRequestStudent
{
    public string StudentName { get; set; }
        = string.Empty;

    public DateTime DateOfBirth { get; set; }

    public Gender Gender { get; set; }

    public Guid ProgramId { get; set; }

    public Guid? CourseId { get; set; }

    public string? PreferredTime { get; set; }

    public string? Remarks { get; set; }
}