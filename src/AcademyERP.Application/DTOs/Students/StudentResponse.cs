using AcademyERP.Domain.Enums;
namespace AcademyERP.Application.DTOs.Students;

public class StudentResponse
{
    public Guid Id { get; set; }

    public string AdmissionNumber { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public DateTime AdmissionDate { get; set; }

    public DateTime DateOfBirth { get; set; }

    public Gender Gender { get; set; }

    public string Country { get; set; } = string.Empty;

    public string TimeZone { get; set; } = string.Empty;

    public string? Remarks { get; set; }
}