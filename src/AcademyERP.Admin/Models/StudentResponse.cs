
using AcademyERP.Domain.Enums;

namespace AcademyERP.Admin.Models;

public class StudentResponse
{
    public Guid Id { get; set; }

    public string AdmissionNumber { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public DateTime AdmissionDate { get; set; }

    public DateTime DateOfBirth { get; set; }

    public string Gender { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public string TimeZone { get; set; } = string.Empty;

    public string? Remarks { get; set; }

    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public List<StudentParentResponse> Parents { get; set; } = new();

    public List<StudentSiblingResponse> Siblings { get; set; } = new();
}

public class StudentParentResponse
{
    public Guid ParentId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public bool IsPrimaryContact { get; set; }
}

public class StudentSiblingResponse
{
    public Guid StudentId { get; set; }

    public string AdmissionNumber { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string Gender { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;
}